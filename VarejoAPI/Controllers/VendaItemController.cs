using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/venda/{vendaId:int}/itens")]
    [ApiController]
    public class VendaItemController : ControllerBase
    {
        private readonly VarejoDbContext _context;
        private readonly IVendaRepository _vendaRepository;

        public VendaItemController(VarejoDbContext context, IVendaRepository vendaRepository)
        {
            _context = context;
            _vendaRepository = vendaRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaItemOutputDTO>>> Get(int vendaId)
        {
            var vendaExiste = await _context.Vendas
                .AnyAsync(v => v.IdVenda == vendaId);

            if (!vendaExiste)
                return NotFound("Venda não encontrada.");

            var itens = await _context.Set<VendaItem>()
                .Include(i => i.Produto)
                .Include(i => i.ProdutoEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .Where(i => i.VendaId == vendaId)
                .OrderBy(i => i.IdVendaItem)
                .ToListAsync();

            return Ok(itens.Select(MapVendaItemOutputDTO));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VendaItemOutputDTO>> Get(int vendaId, int id)
        {
            var item = await ObterItemCompletoAsync(vendaId, id);

            if (item == null)
                return NotFound("Item da venda não encontrado.");

            return Ok(MapVendaItemOutputDTO(item));
        }

        [HttpPost]
        public async Task<ActionResult<VendaItemOutputDTO>> Post(int vendaId, [FromBody] VendaItemInputDTO dto)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.IdVenda == vendaId);

            if (venda == null)
                return NotFound("Venda não encontrada.");

            if (venda.Finalizada)
                return Conflict("Venda finalizada não pode receber novos itens.");

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == dto.ProdutoId);

            if (produto == null)
                return BadRequest("Produto não encontrado.");

            var embalagem = await _context.ProdutosEmbalagem
                .Include(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(pe => pe.IdProdutoEmbalagem == dto.ProdutoEmbalagemId);

            if (embalagem == null)
                return BadRequest("Embalagem não encontrada.");

            if (embalagem.ProdutoId != dto.ProdutoId)
                return BadRequest("A embalagem informada não pertence ao produto.");

            var item = new VendaItem
            {
                VendaId = vendaId,
                ProdutoId = dto.ProdutoId,
                ProdutoEmbalagemId = dto.ProdutoEmbalagemId,
                Quantidade = dto.Quantidade,
                ValorUnitario = dto.ValorUnitario,
                DescontoUnitario = dto.DescontoUnitario
            };

            _context.Set<VendaItem>().Add(item);

            venda.ValorSubtotal += item.Quantidade * item.ValorUnitario;
            venda.DescontoTotal += item.Quantidade * item.DescontoUnitario;

            await _context.SaveChangesAsync();

            var itemCriado = await ObterItemCompletoAsync(vendaId, item.IdVendaItem);
            if (itemCriado == null)
                return StatusCode(500, "Não foi possível carregar o item criado.");

            return CreatedAtAction(
                nameof(Get),
                new { vendaId, id = itemCriado.IdVendaItem },
                MapVendaItemOutputDTO(itemCriado));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<VendaItemOutputDTO>> Put(int vendaId, int id, [FromBody] VendaItemInputDTO dto)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.IdVenda == vendaId);

            if (venda == null)
                return NotFound("Venda não encontrada.");

            if (venda.Finalizada)
                return Conflict("Venda finalizada não pode ser alterada.");

            var item = await _context.Set<VendaItem>()
                .FirstOrDefaultAsync(i => i.IdVendaItem == id && i.VendaId == vendaId);

            if (item == null)
                return NotFound("Item da venda não encontrado.");

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == dto.ProdutoId);

            if (produto == null)
                return BadRequest("Produto não encontrado.");

            var embalagem = await _context.ProdutosEmbalagem
                .FirstOrDefaultAsync(pe => pe.IdProdutoEmbalagem == dto.ProdutoEmbalagemId);

            if (embalagem == null)
                return BadRequest("Embalagem não encontrada.");

            if (embalagem.ProdutoId != dto.ProdutoId)
                return BadRequest("A embalagem informada não pertence ao produto.");

            var subtotalAnteriorBruto = item.Quantidade * item.ValorUnitario;
            var descontoAnteriorTotal = item.Quantidade * item.DescontoUnitario;

            item.ProdutoId = dto.ProdutoId;
            item.ProdutoEmbalagemId = dto.ProdutoEmbalagemId;
            item.Quantidade = dto.Quantidade;
            item.ValorUnitario = dto.ValorUnitario;
            item.DescontoUnitario = dto.DescontoUnitario;

            var subtotalNovoBruto = item.Quantidade * item.ValorUnitario;
            var descontoNovoTotal = item.Quantidade * item.DescontoUnitario;

            venda.ValorSubtotal = venda.ValorSubtotal - subtotalAnteriorBruto + subtotalNovoBruto;
            venda.DescontoTotal = venda.DescontoTotal - descontoAnteriorTotal + descontoNovoTotal;

            await _context.SaveChangesAsync();

            var itemAtualizado = await ObterItemCompletoAsync(vendaId, id);
            if (itemAtualizado == null)
                return StatusCode(500, "Não foi possível carregar o item atualizado.");

            return Ok(MapVendaItemOutputDTO(itemAtualizado));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int vendaId, int id)
        {
            try
            {
                var venda = await _context.Vendas
                    .Include(v => v.Itens)
                    .FirstOrDefaultAsync(v => v.IdVenda == vendaId);

                if (venda == null)
                    return NotFound("Venda não encontrada.");

                if (venda.Finalizada)
                    return Conflict("Venda finalizada não pode ter itens removidos.");

                var item = await _context.Set<VendaItem>()
                    .FirstOrDefaultAsync(i => i.IdVendaItem == id && i.VendaId == vendaId);

                if (item == null)
                    return NotFound("Item da venda não encontrado.");

                venda.ValorSubtotal -= item.Quantidade * item.ValorUnitario;
                venda.DescontoTotal -= item.Quantidade * item.DescontoUnitario;

                _context.Set<VendaItem>().Remove(item);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<VendaItem?> ObterItemCompletoAsync(int vendaId, int id)
        {
            return await _context.Set<VendaItem>()
                .Include(i => i.Produto)
                .Include(i => i.ProdutoEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(i => i.IdVendaItem == id && i.VendaId == vendaId);
        }

        private static VendaItemOutputDTO MapVendaItemOutputDTO(VendaItem item)
        {
            return new VendaItemOutputDTO
            {
                IdVendaItem = item.IdVendaItem,
                ProdutoId = item.ProdutoId,
                NomeProduto = item.Produto?.NomeProduto,
                ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                NomeEmbalagem = item.ProdutoEmbalagem?.TipoEmbalagem?.DescricaoTipoEmbalagem,
                Quantidade = item.Quantidade,
                ValorUnitario = item.ValorUnitario,
                DescontoUnitario = item.DescontoUnitario,
                Subtotal = item.Subtotal
            };
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/inventario/{inventarioId:int}/itens")]
    [ApiController]
    public class InventarioItemController : ControllerBase
    {
        private readonly IInventarioRepository _inventarioRepository;
        private readonly VarejoDbContext _context;

        public InventarioItemController(
            IInventarioRepository inventarioRepository,
            VarejoDbContext context)
        {
            _inventarioRepository = inventarioRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventarioItemOutputDTO>>> Get(int inventarioId)
        {
            var inventarioExiste = await _context.Inventarios
                .AnyAsync(i => i.Id == inventarioId);

            if (!inventarioExiste)
                return NotFound("Inventário não encontrado.");

            var itens = await _context.InventariosItem
                .Include(i => i.Produto)
                .Include(i => i.ProdutoEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .Where(i => i.InventarioId == inventarioId)
                .OrderBy(i => i.Id)
                .ToListAsync();

            return Ok(itens.Select(MapInventarioItemOutputDTO));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<InventarioItemOutputDTO>> Get(int inventarioId, int id)
        {
            var item = await ObterItemCompletoAsync(inventarioId, id);

            if (item == null)
                return NotFound("Item do inventário não encontrado.");

            return Ok(MapInventarioItemOutputDTO(item));
        }

        [HttpPost]
        public async Task<ActionResult<InventarioItemOutputDTO>> Post(int inventarioId, [FromBody] InventarioItemInputDTO dto)
        {
            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.Id == inventarioId);

            if (inventario == null)
                return NotFound("Inventário não encontrado.");

            if (inventario.Finalizado)
                return Conflict("Inventário finalizado não pode receber novos itens.");

            var produto = await _context.Produtos
                .Include(p => p.ProdutosEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(p => p.IdProduto == dto.ProdutoId);

            if (produto == null)
                return NotFound("Produto não encontrado.");

            var embalagem = produto.ProdutosEmbalagem?
                .FirstOrDefault(e => e.IdProdutoEmbalagem == dto.ProdutoEmbalagemId);

            if (embalagem == null || embalagem.TipoEmbalagem == null)
                return BadRequest("Embalagem inválida para o produto informado.");

            var quantidadeConvertida = dto.QuantidadeContada * embalagem.TipoEmbalagem.Multiplicador;

            var item = new InventarioItem
            {
                InventarioId = inventarioId,
                ProdutoId = dto.ProdutoId,
                ProdutoEmbalagemId = dto.ProdutoEmbalagemId,
                QuantidadeSistema = produto.EstoqueAtual,
                QuantidadeContada = quantidadeConvertida,
                ProdutoEmbalagem = embalagem
            };

            try
            {
                await _inventarioRepository.AddItemAsync(item);

                var itemPersistido = await _context.InventariosItem
                    .Include(i => i.Produto)
                    .Include(i => i.ProdutoEmbalagem)
                        .ThenInclude(pe => pe.TipoEmbalagem)
                    .FirstOrDefaultAsync(i =>
                        i.InventarioId == inventarioId &&
                        i.ProdutoId == dto.ProdutoId &&
                        i.ProdutoEmbalagemId == dto.ProdutoEmbalagemId);

                if (itemPersistido == null)
                    return StatusCode(500, "Não foi possível carregar o item salvo.");

                return CreatedAtAction(
                    nameof(Get),
                    new { inventarioId = inventarioId, id = itemPersistido.Id },
                    MapInventarioItemOutputDTO(itemPersistido));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<InventarioItemOutputDTO>> Put(int inventarioId, int id, [FromBody] InventarioItemInputDTO dto)
        {
            var itemAtual = await _context.InventariosItem
                .Include(i => i.ProdutoEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(i => i.Id == id && i.InventarioId == inventarioId);

            if (itemAtual == null)
                return NotFound("Item do inventário não encontrado.");

            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.Id == inventarioId);

            if (inventario == null)
                return NotFound("Inventário não encontrado.");

            if (inventario.Finalizado)
                return Conflict("Inventário finalizado não pode ser alterado.");

            var multiplicador = itemAtual.ProdutoEmbalagem?.TipoEmbalagem?.Multiplicador ?? 1;
            var quantidadeConvertida = dto.QuantidadeContada * multiplicador;

            try
            {
                await _inventarioRepository.AtualizarItemAsync(new InventarioItem
                {
                    Id = id,
                    QuantidadeContada = quantidadeConvertida
                });

                var itemAtualizado = await ObterItemCompletoAsync(inventarioId, id);
                if (itemAtualizado == null)
                    return StatusCode(500, "Não foi possível carregar o item atualizado.");

                return Ok(MapInventarioItemOutputDTO(itemAtualizado));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int inventarioId, int id)
        {
            try
            {
                var item = await _context.InventariosItem
                    .FirstOrDefaultAsync(i => i.Id == id && i.InventarioId == inventarioId);

                if (item == null)
                    return NotFound("Item do inventário não encontrado.");

                var inventario = await _context.Inventarios
                    .FirstOrDefaultAsync(i => i.Id == inventarioId);

                if (inventario == null)
                    return NotFound("Inventário não encontrado.");

                if (inventario.Finalizado)
                    return Conflict("Inventário finalizado não pode ter itens removidos.");

                await _inventarioRepository.RemoverItemAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        private async Task<InventarioItem?> ObterItemCompletoAsync(int inventarioId, int id)
        {
            return await _context.InventariosItem
                .Include(i => i.Produto)
                .Include(i => i.ProdutoEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(i => i.Id == id && i.InventarioId == inventarioId);
        }

        private static InventarioItemOutputDTO MapInventarioItemOutputDTO(InventarioItem item)
        {
            return new InventarioItemOutputDTO
            {
                Id = item.Id,
                InventarioId = item.InventarioId,
                ProdutoId = item.ProdutoId,
                NomeProduto = item.Produto?.NomeProduto,
                ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                DescricaoEmbalagem = item.ProdutoEmbalagem?.TipoEmbalagem?.DescricaoTipoEmbalagem,
                Multiplicador = item.ProdutoEmbalagem?.TipoEmbalagem?.Multiplicador ?? 1,
                QuantidadeSistema = item.QuantidadeSistema,
                QuantidadeContada = item.QuantidadeContada,
                Diferenca = item.Diferenca
            };
        }
    }
}

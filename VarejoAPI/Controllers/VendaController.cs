using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendaController : ControllerBase
    {
        private readonly IVendaRepository _vendaRepository;
        private readonly VarejoDbContext _context;

        public VendaController(
            IVendaRepository vendaRepository,
            VarejoDbContext context)
        {
            _vendaRepository = vendaRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaOutputDTO>>> Get()
        {
            var vendas = await ObterVendasCompletasQuery()
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();

            return Ok(vendas.Select(MapVendaOutputDTO));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VendaOutputDTO>> Get(int id)
        {
            var venda = await ObterVendaCompletaAsync(id);

            if (venda == null)
                return NotFound("Venda não encontrada.");

            return Ok(MapVendaOutputDTO(venda));
        }

        [HttpGet("abertas")]
        public async Task<ActionResult<IEnumerable<VendaOutputDTO>>> GetAbertas()
        {
            var vendas = await ObterVendasCompletasQuery()
                .Where(v => !v.Finalizada)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();

            return Ok(vendas.Select(MapVendaOutputDTO));
        }

        [HttpGet("finalizadas")]
        public async Task<ActionResult<IEnumerable<VendaOutputDTO>>> GetFinalizadas()
        {
            var vendas = await ObterVendasCompletasQuery()
                .Where(v => v.Finalizada)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();

            return Ok(vendas.Select(MapVendaOutputDTO));
        }

        [HttpPost]
        public async Task<ActionResult<VendaOutputDTO>> Post([FromBody] VendaInputDTO dto)
        {
            var pessoaExiste = await _context.Pessoas
                .AnyAsync(p => p.IdPessoa == dto.PessoaId);

            if (!pessoaExiste)
                return BadRequest("Cliente não encontrado.");

            var formaPagamentoExiste = await _context.FormasPagamento
                .AnyAsync(f => f.IdFormaPagamento == dto.FormaPagamentoId);

            if (!formaPagamentoExiste)
                return BadRequest("Forma de pagamento não encontrada.");

            if (dto.PrazoPagamentoId.HasValue)
            {
                var prazoExiste = await _context.PrazosPagamento
                    .AnyAsync(p => p.IdPrazoPagamento == dto.PrazoPagamentoId.Value);

                if (!prazoExiste)
                    return BadRequest("Prazo de pagamento não encontrado.");
            }

            var venda = new Venda
            {
                PessoaId = dto.PessoaId,
                FormaPagamentoId = dto.FormaPagamentoId,
                PrazoPagamentoId = dto.PrazoPagamentoId,
                DataVenda = DateTime.Now,
                Observacao = dto.Observacao,
                ValorSubtotal = 0,
                DescontoTotal = dto.DescontoTotal,
                Finalizada = false
            };

            await _vendaRepository.CriarPedidoAsync(venda);

            var vendaCriada = await ObterVendaCompletaAsync(venda.IdVenda);
            if (vendaCriada == null)
                return StatusCode(500, "Não foi possível carregar a venda criada.");

            return CreatedAtAction(
                nameof(Get),
                new { id = vendaCriada.IdVenda },
                MapVendaOutputDTO(vendaCriada));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] VendaInputDTO dto)
        {
            var venda = await _context.Vendas
                .FirstOrDefaultAsync(v => v.IdVenda == id);

            if (venda == null)
                return NotFound("Venda não encontrada.");

            if (venda.Finalizada)
                return Conflict("Venda finalizada não pode ser editada.");

            var pessoaExiste = await _context.Pessoas
                .AnyAsync(p => p.IdPessoa == dto.PessoaId);

            if (!pessoaExiste)
                return BadRequest("Cliente não encontrado.");

            var formaPagamentoExiste = await _context.FormasPagamento
                .AnyAsync(f => f.IdFormaPagamento == dto.FormaPagamentoId);

            if (!formaPagamentoExiste)
                return BadRequest("Forma de pagamento não encontrada.");

            if (dto.PrazoPagamentoId.HasValue)
            {
                var prazoExiste = await _context.PrazosPagamento
                    .AnyAsync(p => p.IdPrazoPagamento == dto.PrazoPagamentoId.Value);

                if (!prazoExiste)
                    return BadRequest("Prazo de pagamento não encontrado.");
            }

            venda.PessoaId = dto.PessoaId;
            venda.FormaPagamentoId = dto.FormaPagamentoId;
            venda.PrazoPagamentoId = dto.PrazoPagamentoId;
            venda.Observacao = dto.Observacao;
            venda.DescontoTotal = dto.DescontoTotal;

            await _vendaRepository.AtualizarPedidoAsync(venda);

            return NoContent();
        }

        [HttpPost("{id:int}/faturar")]
        public async Task<ActionResult<VendaOutputDTO>> Faturar(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.IdVenda == id);

            if (venda == null)
                return NotFound("Venda não encontrada.");

            if (venda.Finalizada)
                return Conflict("Venda já está finalizada.");

            if (venda.Itens == null || !venda.Itens.Any())
                return BadRequest("A venda precisa ter pelo menos um item para ser faturada.");

            try
            {
                var sucesso = await _vendaRepository.FaturarVendaAsync(id);

                if (!sucesso)
                    return BadRequest("Não foi possível faturar a venda.");

                var vendaFaturada = await ObterVendaCompletaAsync(id);
                if (vendaFaturada == null)
                    return StatusCode(500, "Venda faturada, mas não foi possível recarregar os dados.");

                return Ok(MapVendaOutputDTO(vendaFaturada));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var venda = await _context.Vendas
                .FirstOrDefaultAsync(v => v.IdVenda == id);

            if (venda == null)
                return NotFound("Venda não encontrada.");

            if (venda.Finalizada)
                return Conflict("Venda finalizada não pode ser cancelada/removida.");

            try
            {
                var sucesso = await _vendaRepository.CancelarVendaAsync(id);

                if (!sucesso)
                    return BadRequest("Não foi possível cancelar a venda.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private IQueryable<Venda> ObterVendasCompletasQuery()
        {
            return _context.Vendas
                .Include(v => v.Pessoa)
                .Include(v => v.FormaPagamento)
                .Include(v => v.PrazoPagamento)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.ProdutoEmbalagem)
                        .ThenInclude(pe => pe.TipoEmbalagem);
        }

        private async Task<Venda?> ObterVendaCompletaAsync(int id)
        {
            return await ObterVendasCompletasQuery()
                .FirstOrDefaultAsync(v => v.IdVenda == id);
        }

        private static VendaOutputDTO MapVendaOutputDTO(Venda venda)
        {
            return new VendaOutputDTO
            {
                IdVenda = venda.IdVenda,
                DataVenda = venda.DataVenda,
                ValorSubtotal = venda.Itens?.Sum(i => i.Quantidade * i.ValorUnitario) ?? venda.ValorSubtotal,
                DescontoTotal = venda.Itens?.Sum(i => i.Quantidade * i.DescontoUnitario) ?? venda.DescontoTotal,
                ValorFinal = (venda.Itens?.Sum(i => i.Quantidade * i.ValorUnitario) ?? venda.ValorSubtotal)
                           - (venda.Itens?.Sum(i => i.Quantidade * i.DescontoUnitario) ?? venda.DescontoTotal),
                Observacao = venda.Observacao,
                Finalizada = venda.Finalizada,
                PessoaId = venda.PessoaId,
                NomeCliente = venda.Pessoa?.NomeRazao,
                FormaPagamentoId = venda.FormaPagamentoId,
                FormaPagamento = venda.FormaPagamento?.DescricaoFormaPagamento,
                PrazoPagamentoId = venda.PrazoPagamentoId,
                PrazoPagamento = venda.PrazoPagamento?.Descricao,
                Itens = venda.Itens?
                    .Select(MapVendaItemOutputDTO)
                    .ToList() ?? new List<VendaItemOutputDTO>()
            };
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

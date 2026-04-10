using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Models;
using VarejoSHARED.DTO;

namespace Varejo.Controllers.Api
{
    [ApiController]
    [Route("api/estoque")]
    public class HistoricoProdutoController : ControllerBase
    {
        private readonly VarejoDbContext _context;

        public HistoricoProdutoController(VarejoDbContext context)
        {
            _context = context;
        }

        [HttpGet("{produtoId}/historico")]
        public async Task<IActionResult> GetHistorico(int produtoId)
        {
            try
            {
                if (produtoId <= 0)
                    return BadRequest("Id inválido.");

                var produtoExiste = await _context.Produtos
                    .AnyAsync(p => p.IdProduto == produtoId);

                if (!produtoExiste)
                    return NotFound("Produto não encontrado");

                var historico = await _context.HistoricosProduto
                    .Include(h => h.TipoMovimento)
                    .Include(h => h.EspecieMovimento)
                    .Where(h => h.ProdutoId == produtoId)
                    .OrderByDescending(h => h.Data)
                    .ThenByDescending(h => h.Id)
                    .ToListAsync();

                if (!historico.Any())
                    return NotFound("Nenhum histórico encontrado para este produto");

                var output = historico.Select(h => new HistoricoProdutoOutput
                {
                    Id = h.Id,
                    Data = h.Data,
                    TipoMovimento = h.TipoMovimento?.DescricaoTipoMovimento,
                    EspecieMovimento = h.EspecieMovimento?.DescricaoEspecieMovimento,
                    EstoqueAntes = h.EstoqueAntes,
                    QuantidadeMovimento = h.QuantidadeMovimento,
                    EstoqueDepois = h.EstoqueDepois,
                    Observacao = h.Observacao
                }).ToList();

                return Ok(output);
            }
            catch (Exception ex)
            {
                // Logar o erro para diagnóstico
                Console.WriteLine($"Erro em GetHistorico: {ex.Message}");
                return StatusCode(500, "Erro interno ao buscar histórico do produto.");
            }
        }
    }
}

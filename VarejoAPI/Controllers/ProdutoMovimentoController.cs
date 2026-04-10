using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Models;
using VarejoSHARED.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoMovimentoController : ControllerBase
    {
        private readonly VarejoDbContext _context;

        public ProdutoMovimentoController(VarejoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoMovimentoOutputDTO>>> Get()
        {
            var produtosMovimento = await _context.ProdutosMovimento
                .Include(pm => pm.Produto)
                .Include(pm => pm.ProdutoEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .OrderByDescending(pm => pm.IdProdutoMovimento)
                .ToListAsync();

            return Ok(produtosMovimento.Select(MapProdutoMovimentoOutputDTO));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProdutoMovimentoOutputDTO>> Get(int id)
        {
            var produtoMovimento = await ObterProdutoMovimentoCompletoAsync(id);

            if (produtoMovimento == null)
                return NotFound();

            return Ok(MapProdutoMovimentoOutputDTO(produtoMovimento));
        }

        [HttpGet("movimento/{movimentoId:int}")]
        public async Task<ActionResult<IEnumerable<ProdutoMovimentoOutputDTO>>> GetByMovimento(int movimentoId)
        {
            var existeMovimento = await _context.Movimentos
                .AnyAsync(m => m.IdMovimento == movimentoId);

            if (!existeMovimento)
                return NotFound("Movimento não encontrado.");

            var produtosMovimento = await _context.ProdutosMovimento
                .Include(pm => pm.Produto)
                .Include(pm => pm.ProdutoEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .Where(pm => pm.MovimentoId == movimentoId)
                .OrderBy(pm => pm.IdProdutoMovimento)
                .ToListAsync();

            return Ok(produtosMovimento.Select(MapProdutoMovimentoOutputDTO));
        }

        private async Task<ProdutoMovimento?> ObterProdutoMovimentoCompletoAsync(int id)
        {
            return await _context.ProdutosMovimento
                .Include(pm => pm.Produto)
                .Include(pm => pm.ProdutoEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(pm => pm.IdProdutoMovimento == id);
        }

        private static ProdutoMovimentoOutputDTO MapProdutoMovimentoOutputDTO(ProdutoMovimento produtoMovimento)
        {
            return new ProdutoMovimentoOutputDTO
            {
                IdProdutoMovimento = produtoMovimento.IdProdutoMovimento,
                Quantidade = produtoMovimento.Quantidade,
                Produto = produtoMovimento.Produto?.NomeProduto ?? string.Empty,
                Embalagem = produtoMovimento.ProdutoEmbalagem?.TipoEmbalagem?.DescricaoTipoEmbalagem ?? string.Empty
            };
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Varejo.Data;
using VarejoAPI.DTO;
using VarejoAPI.Services;
using VarejoSHARED.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelatorioController : ControllerBase
    {
        private readonly VarejoDbContext _context;

        public RelatorioController(VarejoDbContext context)
        {
            _context = context;
        }

        #region RELATÓRIO 101 - PRODUTOS CATEGORIZADOS

        [HttpPost("101/dados")]
        public async Task<ActionResult<List<ProdutoDTO>>> GetProdutos101([FromBody] RelatorioFiltroProdutosDTO filtro)
        {
            var query = _context.Produtos
                .Include(p => p.Familia).ThenInclude(f => f.Categoria)
                .Include(p => p.Familia).ThenInclude(f => f.Marca)
                .AsNoTracking()
                .AsQueryable();

            // 1. Filtro de Texto (Herdado de RelatorioFiltroBaseDTO)
            if (!string.IsNullOrWhiteSpace(filtro.TermoBusca))
                query = query.Where(p => p.NomeProduto.Contains(filtro.TermoBusca));

            // 2. Filtros Multi-Select
            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(p => p.Familia != null && filtro.CategoriasIds.Contains(p.Familia.CategoriaId));

            if (filtro.MarcasIds != null && filtro.MarcasIds.Any())
                query = query.Where(p => p.Familia != null && p.Familia.MarcaId.HasValue && filtro.MarcasIds.Contains(p.Familia.MarcaId.Value));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(p => p.FamiliaId > 0 && filtro.FamiliasIds.Contains(p.FamiliaId));

            // 3. Filtro de Status
            if (filtro.Ativo.HasValue)
                query = query.Where(p => p.Ativo == filtro.Ativo.Value);

            // 4. Projeção usando a sua ProdutoDTO original
            var lista = await query
                .Select(p => new ProdutoDTO
                {
                    IdProduto = p.IdProduto,
                    NomeProduto = p.NomeProduto ?? "Sem Nome",
                    DescricaoCategoria = p.Familia != null && p.Familia.Categoria != null ? p.Familia.Categoria.DescricaoCategoria : "Sem Categoria",
                    NomeMarca = p.Familia != null && p.Familia.Marca != null ? p.Familia.Marca.NomeMarca : "Sem Marca",
                    NomeFamilia = p.Familia != null ? p.Familia.NomeFamilia : "Sem Família",
                    EstoqueAtual = p.EstoqueAtual,
                    CustoMedio = p.CustoMedio,
                    Ativo = p.Ativo,
                    UrlImagem = p.UrlImagem ?? ""
                })
                .OrderBy(p => p.NomeProduto)
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost("101/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio101([FromBody] RelatorioFiltroProdutosDTO filtro)
        {
            // 1. Busca os dados usando a mesma lógica da tela
            var actionResult = await GetProdutos101(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var listaProdutos = okResult?.Value as List<ProdutoDTO>;

            // 2. Valida se tem dados
            if (listaProdutos == null || !listaProdutos.Any())
                return BadRequest("Nenhum produto encontrado com os filtros informados para gerar o PDF.");

            // 3. Chama o SEU serviço original de PDF
            var service = new RelatorioExportService();
            var pdfBytes = service.GerarPdfProdutos(listaProdutos);

            // 4. Devolve o arquivo para o navegador baixar
            return File(pdfBytes, "application/pdf", "RelatorioProdutos.pdf");
        }

        #endregion

        #region MÓDULO 300 - MOVIMENTAÇÕES (Legado/Em Adaptação)
        // Mantive intacto para não quebrar outras partes do seu sistema
        [HttpPost("movimentacoes")]
        public async Task<ActionResult<List<MovimentoOutputDTO>>> GetMovimentacoes(RelatorioFiltroMovimentacaoDTO filtro)
        {
            var query = _context.Movimentos.AsNoTracking();

            if (filtro.DataInicio.HasValue)
                query = query.Where(m => m.DataMovimento >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(m => m.DataMovimento <= filtro.DataFim.Value);

            if (filtro.IdPessoa.HasValue)
                query = query.Where(m => m.PessoaId == filtro.IdPessoa);

            var movimentos = await query
                 .OrderByDescending(m => m.DataMovimento)
                 .Select(m => new MovimentoOutputDTO
                 {
                     IdMovimento = m.IdMovimento,
                     Pessoa = m.Pessoa != null ? m.Pessoa.NomeRazao : "Consumidor",
                     TipoMovimento = m.TipoMovimento != null ? m.TipoMovimento.DescricaoTipoMovimento : "Geral",
                     DataMovimento = m.DataMovimento,
                     Produtos = m.ProdutosMovimento.Select(p => new ProdutoMovimentoOutputDTO
                     {
                         IdProdutoMovimento = p.IdProdutoMovimento,
                         Quantidade = p.Quantidade,
                         Produto = p.Produto != null ? p.Produto.NomeProduto : "",
                         Embalagem = p.ProdutoEmbalagem != null && p.ProdutoEmbalagem.TipoEmbalagem != null
                                     ? p.ProdutoEmbalagem.TipoEmbalagem.DescricaoTipoEmbalagem : "Sem Embalagem"
                     }).ToList()
                 })
                 .ToListAsync();

            return Ok(movimentos);
        }
        #endregion

        #region FAVORITOS
        [HttpPost("favoritar")]
        public async Task<IActionResult> ToggleFavorito([FromBody] RelatorioFavoritoDTO dto)
        {
            var favoritoExistente = await _context.UsuarioRelatoriosFavoritos
                .FirstOrDefaultAsync(f => f.UsuarioId == dto.IdUsuario && f.CodigoRelatorio == dto.CodigoRelatorio);

            if (favoritoExistente != null)
            {
                _context.UsuarioRelatoriosFavoritos.Remove(favoritoExistente);
                await _context.SaveChangesAsync();
                return Ok(false);
            }

            _context.UsuarioRelatoriosFavoritos.Add(new UsuarioRelatorioFavorito
            {
                UsuarioId = dto.IdUsuario,
                CodigoRelatorio = dto.CodigoRelatorio
            });

            await _context.SaveChangesAsync();
            return Ok(true);
        }

        [HttpGet("meus-favoritos/{usuarioId}")]
        public async Task<ActionResult<List<int>>> GetMeusFavoritos(int usuarioId)
        {
            var codigos = await _context.UsuarioRelatoriosFavoritos
                .Where(f => f.UsuarioId == usuarioId)
                .Select(f => f.CodigoRelatorio)
                .ToListAsync();

            return Ok(codigos);
        }
        #endregion
    }
}
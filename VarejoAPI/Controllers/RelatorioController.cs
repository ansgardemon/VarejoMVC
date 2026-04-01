using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using VarejoSHARED.DTO;
using VarejoAPI.Services;
using Varejo.Models;

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

            if (!string.IsNullOrWhiteSpace(filtro.TermoBusca))
                query = query.Where(p => p.NomeProduto.Contains(filtro.TermoBusca));

            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(p => p.Familia != null && filtro.CategoriasIds.Contains(p.Familia.CategoriaId));

            if (filtro.MarcasIds != null && filtro.MarcasIds.Any())
                query = query.Where(p => p.Familia != null && p.Familia.MarcaId != null && filtro.MarcasIds.Contains(p.Familia.MarcaId.Value));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(p => filtro.FamiliasIds.Contains(p.FamiliaId));

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(p => filtro.ProdutosIds.Contains(p.IdProduto));

            if (filtro.Ativo.HasValue)
                query = query.Where(p => p.Ativo == filtro.Ativo.Value);

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
            var actionResult = await GetProdutos101(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var listaProdutos = okResult?.Value as List<ProdutoDTO>;

            if (listaProdutos == null || !listaProdutos.Any())
                return BadRequest("Nenhum produto encontrado com os filtros informados para gerar o PDF.");

            var service = new RelatorioExportService();
            var pdfBytes = service.GerarPdfProdutos(listaProdutos);

            return File(pdfBytes, "application/pdf", "RelatorioProdutos.pdf");
        }

        #endregion

        #region RELATÓRIO 102 - PRODUTOS POR VALORES

        [HttpPost("102/dados")]
        public async Task<ActionResult<List<Relatorio102DTO>>> GetProdutos102([FromBody] RelatorioFiltroProdutosDTO filtro)
        {
            var query = _context.ProdutosEmbalagem
                .Include(pe => pe.Produto)
                .Include(pe => pe.TipoEmbalagem)
                .AsNoTracking()
                .AsQueryable();

            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(pe => pe.Produto.Familia != null && filtro.CategoriasIds.Contains(pe.Produto.Familia.CategoriaId));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(pe => filtro.FamiliasIds.Contains(pe.Produto.FamiliaId));

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(pe => filtro.ProdutosIds.Contains(pe.ProdutoId));

            if (filtro.Ativo.HasValue)
                query = query.Where(pe => pe.Produto.Ativo == filtro.Ativo.Value);

            var lista = await query
                .Select(pe => new Relatorio102DTO
                {
                    IdProduto = pe.ProdutoId,
                    NomeProduto = pe.Produto.NomeProduto,
                    Embalagem = pe.TipoEmbalagem.DescricaoTipoEmbalagem,
                    CustoMedio = pe.Produto.CustoMedio,
                    PrecoVenda = pe.Preco
                })
                .OrderBy(p => p.NomeProduto)
                .ThenBy(p => p.PrecoVenda)
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost("102/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio102([FromBody] RelatorioFiltroProdutosDTO filtro)
        {
            var actionResult = await GetProdutos102(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var lista102 = okResult?.Value as List<Relatorio102DTO>;

            if (lista102 == null || !lista102.Any())
                return BadRequest("Nenhum registro encontrado para gerar o PDF.");

            var service = new RelatorioExportService();
            var pdfBytes = service.GerarPdfProdutosValores(lista102);

            return File(pdfBytes, "application/pdf", "RelatorioProdutosValores.pdf");
        }
        #endregion

        #region RELATÓRIO 103 - MOVIMENTO DE ESTOQUE POR PRODUTO

        [HttpPost("103/dados")]
        public async Task<ActionResult<List<Relatorio103DTO>>> GetDadosRelatorio103([FromBody] RelatorioFiltro103DTO filtro)
        {
            var query = _context.ProdutosMovimento
                .Include(pm => pm.Movimento).ThenInclude(m => m.TipoMovimento)
                .Include(pm => pm.Movimento).ThenInclude(m => m.Pessoa)
                .Include(pm => pm.Produto)
                .Include(pm => pm.ProdutoEmbalagem).ThenInclude(pe => pe.TipoEmbalagem)
                .AsNoTracking()
                .AsQueryable();

            if (filtro.DataInicio.HasValue)
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento <= filtro.DataFim.Value);

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(pm => filtro.ProdutosIds.Contains(pm.ProdutoId));

            var lista = await query
                .Select(pm => new Relatorio103DTO
                {
                    IdMovimento = pm.MovimentoId,
                    DataMovimento = pm.Movimento != null ? pm.Movimento.DataMovimento : System.DateTime.MinValue,
                    TipoMovimento = pm.Movimento != null && pm.Movimento.TipoMovimento != null
                                    ? pm.Movimento.TipoMovimento.DescricaoTipoMovimento
                                    : "Geral",
                    Pessoa = pm.Movimento != null && pm.Movimento.Pessoa != null
                             ? pm.Movimento.Pessoa.NomeRazao
                             : "Não Informado",
                    ProdutoId = pm.ProdutoId,
                    ProdutoNome = pm.Produto != null ? pm.Produto.NomeProduto : "Desconhecido",
                    Quantidade = pm.Quantidade,
                    Embalagem = pm.ProdutoEmbalagem != null && pm.ProdutoEmbalagem.TipoEmbalagem != null
                                ? pm.ProdutoEmbalagem.TipoEmbalagem.DescricaoTipoEmbalagem
                                : "Un."
                })
                .OrderByDescending(x => x.DataMovimento)
                .ToListAsync();

            return Ok(lista);
        }


        // ADICIONADO: Exportação do Relatório 103
        [HttpPost("103/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio103([FromBody] RelatorioFiltro103DTO filtro)
        {
            var actionResult = await GetDadosRelatorio103(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var lista103 = okResult?.Value as List<Relatorio103DTO>;

            if (lista103 == null || !lista103.Any())
                return BadRequest("Nenhum registro encontrado para gerar o PDF.");

            var service = new RelatorioExportService();
            // NECESSITA IMPLEMENTAÇÃO no RelatorioExportService
            var pdfBytes = service.GerarPdfRelatorio103(lista103);

            return File(pdfBytes, "application/pdf", "RelatorioMovimentoEstoque.pdf");
        }

        #endregion

        #region MÓDULO 300 - MOVIMENTAÇÕES (Legado/Em Adaptação)

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

        // ADICIONADO: Exportação de Movimentações (Módulo 300)
        /*
        [HttpPost("movimentacoes/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfMovimentacoes([FromBody] RelatorioFiltroMovimentacaoDTO filtro)
        {
            var actionResult = await GetMovimentacoes(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var listaMovimentos = okResult?.Value as List<MovimentoOutputDTO>;

            if (listaMovimentos == null || !listaMovimentos.Any())
                return BadRequest("Nenhum registro encontrado para gerar o PDF.");

            var service = new RelatorioExportService();
            // NECESSITA IMPLEMENTAÇÃO no RelatorioExportService
            var pdfBytes = service.GerarPdfMovimentacoes(listaMovimentos);

            return File(pdfBytes, "application/pdf", "RelatorioMovimentacoes.pdf");
        }
        */
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
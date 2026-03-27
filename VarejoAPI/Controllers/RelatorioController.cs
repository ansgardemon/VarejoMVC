using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using VarejoAPI.Services;
using VarejoSHARED.DTO;

[Route("api/[controller]")]
[ApiController]
public class RelatorioController : ControllerBase
{
    private readonly VarejoDbContext _context;

    public RelatorioController(VarejoDbContext context)
    {
        _context = context;
    }

    // 1. MÉTODO DE BUSCA (LIMPO E SEM RECURSÃO)
    [HttpPost("produtos")]
    public async Task<ActionResult<List<ProdutoDTO>>> GetProdutos(RelatorioFiltroProdutosDTO filtro)
    {
        var query = _context.Produtos
            .Include(p => p.Familia)
                .ThenInclude(f => f.Categoria)
            .Include(p => p.Familia)
                .ThenInclude(f => f.Marca) // Garante que a marca venha na query
            .AsNoTracking()
            .AsQueryable();

        // Filtros
        if (!string.IsNullOrWhiteSpace(filtro.TermoBusca))
            query = query.Where(p => p.NomeProduto.Contains(filtro.TermoBusca));

        if (filtro.IdCategoria.HasValue && filtro.IdCategoria > 0)
            query = query.Where(p => p.Familia.CategoriaId == filtro.IdCategoria);

        if (filtro.IdMarca.HasValue && filtro.IdMarca > 0)
            query = query.Where(p => p.Familia.MarcaId == filtro.IdMarca);

        if (filtro.Ativo.HasValue)
            query = query.Where(p => p.Ativo == filtro.Ativo.Value);

        // PROJEÇÃO BLINDADA (Usando ?. para evitar NullReference)
        var lista = await query
            .Select(p => new ProdutoDTO
            {
                IdProduto = p.IdProduto,
                NomeProduto = p.NomeProduto ?? "Sem Nome",
                DescricaoCategoria = p.Familia != null && p.Familia.Categoria != null
                                     ? p.Familia.Categoria.DescricaoCategoria
                                     : "Sem Categoria",
                NomeMarca = p.Familia != null && p.Familia.Marca != null
                            ? p.Familia.Marca.NomeMarca
                            : "Sem Marca",
                EstoqueAtual = p.EstoqueAtual,
                CustoMedio = p.CustoMedio,
                Ativo = p.Ativo
            })
            .ToListAsync();

        return Ok(lista);
    }

    // 2. MÉTODO DE EXPORTAÇÃO (CHAMA A BUSCA CORRETAMENTE)
    [HttpPost("exportar/pdf")]
    public async Task<IActionResult> ExportarPdf(RelatorioFiltroProdutosDTO filtro)
    {
        // Chama o método acima e extrai o resultado
        var actionResult = await GetProdutos(filtro);
        var okResult = actionResult.Result as OkObjectResult;
        var listaProdutos = okResult?.Value as List<ProdutoDTO>;

        if (listaProdutos == null || !listaProdutos.Any())
        {
            return BadRequest("Nenhum produto encontrado para gerar o PDF.");
        }

        var service = new RelatorioExportService();
        var pdfBytes = service.GerarPdfProdutos(listaProdutos);

        return File(pdfBytes, "application/pdf", "RelatorioProdutos.pdf");
    }

    // 3. MOVIMENTAÇÕES (AJUSTADO)
    [HttpPost("movimentacoes")]
    public async Task<ActionResult<List<MovimentoItemDTO>>> GetMovimentacoes(RelatorioFiltroMovimentacaoDTO filtro)
    {
        var query = _context.Movimentos
            .Include(m => m.Pessoa)
            .Include(m => m.TipoMovimento)
            .Include(m => m.ProdutosMovimento)
            .AsNoTracking();

        if (filtro.DataInicio.HasValue)
            query = query.Where(m => m.DataMovimento >= filtro.DataInicio.Value);

        if (filtro.DataFim.HasValue)
            query = query.Where(m => m.DataMovimento <= filtro.DataFim.Value);

        if (filtro.IdPessoa.HasValue)
            query = query.Where(m => m.PessoaId == filtro.IdPessoa);

        var resultado = await query
            .Select(m => new MovimentoItemDTO
            {
                IdMovimento = m.IdMovimento,
                Pessoa = m.Pessoa != null ? m.Pessoa.NomeRazao : "Consumidor",
                TipoMovimento = m.TipoMovimento != null ? m.TipoMovimento.DescricaoTipoMovimento : "Geral",
                Data = m.DataMovimento,
                QtdeProdutos = m.ProdutosMovimento != null ? m.ProdutosMovimento.Count : 0
            })
            .OrderByDescending(m => m.Data)
            .ToListAsync();

        return Ok(resultado);
    }
}
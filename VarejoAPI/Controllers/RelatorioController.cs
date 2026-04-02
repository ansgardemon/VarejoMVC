using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using VarejoSHARED.DTO;
using VarejoAPI.Services;
using Varejo.Models;
using VarejoSHARED.DTO.Relatorios;

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
            var pdfBytes = service.GerarPdfRelatorio101(listaProdutos);

            return File(pdfBytes, "application/pdf", "RelatorioProdutos.pdf");
        }

        #endregion

        #region RELATÓRIO 102 - PRECIFICAÇÃO E MARGENS DE LUCRO

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
            var pdfBytes = service.GerarPdfRelatorio102(lista102);

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

        #region RELATÓRIO 104 - CURVA ABC DE PRODUTOS

        [HttpPost("104/dados")]
        public async Task<ActionResult<List<Relatorio104DTO>>> GetDadosRelatorio104([FromBody] RelatorioFiltroProdutosDTO filtro)
        {
            // 1. Busca os movimentos (Geralmente filtra-se por espécie de Saída/Venda, 
            // mas aqui consideraremos que os preços nas embalagens representam as vendas)
            var query = _context.ProdutosMovimento
                .Include(pm => pm.Produto).ThenInclude(p => p.Familia).ThenInclude(f => f.Categoria)
                .Include(pm => pm.ProdutoEmbalagem)
                .Include(pm => pm.Movimento)
                .AsNoTracking()
                .AsQueryable();

            // Filtros de Data (Herdados da base)
            if (filtro.DataInicio.HasValue)
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento <= filtro.DataFim.Value);

            // Filtros de Produtos (Multiselect)
            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(pm => pm.Produto.Familia != null && filtro.CategoriasIds.Contains(pm.Produto.Familia.CategoriaId));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(pm => filtro.FamiliasIds.Contains(pm.Produto.FamiliaId));

            if (filtro.MarcasIds != null && filtro.MarcasIds.Any())
                query = query.Where(pm => pm.Produto.Familia != null && pm.Produto.Familia.MarcaId != null && filtro.MarcasIds.Contains(pm.Produto.Familia.MarcaId.Value));

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(pm => filtro.ProdutosIds.Contains(pm.ProdutoId));

            // 2. Agrupamento e Soma (Fazemos um Select prévio para garantir que o EF Core traduza para SQL perfeitamente)
            var dadosAgrupados = await query
                .Select(pm => new
                {
                    ProdutoId = pm.ProdutoId,
                    NomeProduto = pm.Produto.NomeProduto,
                    Categoria = pm.Produto.Familia != null && pm.Produto.Familia.Categoria != null ? pm.Produto.Familia.Categoria.DescricaoCategoria : "Sem Categoria",
                    Quantidade = Math.Abs(pm.Quantidade), // Garante número positivo
                    ValorTotal = Math.Abs(pm.Quantidade) * (pm.ProdutoEmbalagem != null ? pm.ProdutoEmbalagem.Preco : 0)
                })
                .GroupBy(x => new { x.ProdutoId, x.NomeProduto, x.Categoria })
                .Select(g => new
                {
                    IdProduto = g.Key.ProdutoId,
                    Nome = g.Key.NomeProduto,
                    Categoria = g.Key.Categoria,
                    Qtd = g.Sum(x => x.Quantidade),
                    Faturamento = g.Sum(x => x.ValorTotal)
                })
                .Where(x => x.Faturamento > 0)
                .OrderByDescending(x => x.Faturamento) // IMPORTANTE: Curva ABC DEVE ser ordenada do maior para o menor
                .ToListAsync();

            // 3. Cálculos da Curva ABC
            var totalFaturamento = dadosAgrupados.Sum(x => x.Faturamento);
            decimal acumulado = 0;
            var resultado = new List<Relatorio104DTO>();

            foreach (var item in dadosAgrupados)
            {
                acumulado += item.Faturamento;

                // Regra de 3 para o percentual acumulado
                decimal percentual = totalFaturamento > 0 ? (acumulado / totalFaturamento) * 100 : 0;

                // Classificação: Até 80% é A | de 80.1 a 95% é B | O resto é C
                string classe = percentual <= 80 ? "A" : (percentual <= 95 ? "B" : "C");

                resultado.Add(new Relatorio104DTO
                {
                    IdProduto = item.IdProduto,
                    NomeProduto = item.Nome,
                    Categoria = item.Categoria,
                    QuantidadeVendida = item.Qtd,
                    Faturamento = item.Faturamento,
                    PercentualAcumulado = percentual,
                    ClasseABC = classe
                });
            }

            return Ok(resultado);
        }

        [HttpPost("104/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio104([FromBody] RelatorioFiltroProdutosDTO filtro)
        {
            // 1. Busca os dados usando a mesma lógica que já construímos
            var actionResult = await GetDadosRelatorio104(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var listaDados = okResult?.Value as List<Relatorio104DTO>;

            if (listaDados == null || !listaDados.Any())
                return BadRequest("Nenhum dado encontrado no período selecionado para gerar a Curva ABC.");

            // 2. Chama o serviço do QuestPDF
            var service = new RelatorioExportService();
            var pdfBytes = service.GerarPdfRelatorio104(listaDados);

            return File(pdfBytes, "application/pdf", "RelatorioCurvaABC.pdf");
        }

        #endregion

        #region RELATÓRIO 105 - PRODUTOS SEM GIRO

        [HttpPost("105/dados")]
        public async Task<ActionResult<List<Relatorio105DTO>>> GetDadosRelatorio105([FromBody] RelatorioFiltro105DTO filtro)
        {
            // Calcula a data limite. Ex: Hoje menos 30 dias.
            var dataCorte = DateTime.Now.Date.AddDays(-filtro.DiasSemGiro);

            var query = _context.Produtos
                .Include(p => p.Familia).ThenInclude(f => f.Categoria)
                .AsNoTracking()
                .AsQueryable();

            // Filtros Padrão
            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(p => p.Familia != null && filtro.CategoriasIds.Contains(p.Familia.CategoriaId));

            if (filtro.MarcasIds != null && filtro.MarcasIds.Any())
                query = query.Where(p => p.Familia != null && p.Familia.MarcaId != null && filtro.MarcasIds.Contains(p.Familia.MarcaId.Value));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(p => filtro.FamiliasIds.Contains(p.FamiliaId));

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(p => filtro.ProdutosIds.Contains(p.IdProduto));

            // Apenas produtos ativos por padrão (não faz sentido analisar giro de produto desativado)
            var ativo = filtro.Ativo ?? true;
            query = query.Where(p => p.Ativo == ativo);

            // Seleciona o produto e descobre quando foi a última movimentação dele
            var listaCrua = await query
                .Select(p => new
                {
                    Produto = p,
                    UltimaVenda = _context.ProdutosMovimento
                        .Where(pm => pm.ProdutoId == p.IdProduto && pm.Movimento != null)
                        .Max(pm => (DateTime?)pm.Movimento.DataMovimento)
                })
                // Filtro Principal: Se a última venda for NULA (nunca vendeu) ou MENOR que a data de corte
                .Where(x => x.UltimaVenda == null || x.UltimaVenda <= dataCorte)
                .ToListAsync();

            // Mapeamento em memória para calcular os dias exatos
            var resultado = listaCrua.Select(x => new Relatorio105DTO
            {
                IdProduto = x.Produto.IdProduto,
                NomeProduto = x.Produto.NomeProduto ?? "Sem Nome",
                Categoria = x.Produto.Familia != null && x.Produto.Familia.Categoria != null ? x.Produto.Familia.Categoria.DescricaoCategoria : "Sem Categoria",
                EstoqueAtual = x.Produto.EstoqueAtual,
                UltimaVenda = x.UltimaVenda,
                // Se nunca vendeu, usamos um número alto ou 999 para identificar no layout
                DiasParado = x.UltimaVenda.HasValue ? (int)(DateTime.Now.Date - x.UltimaVenda.Value.Date).TotalDays : 999
            })
            .OrderByDescending(x => x.DiasParado) // Os que estão parados há mais tempo aparecem primeiro
            .ToList();

            return Ok(resultado);
        }

        [HttpPost("105/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio105([FromBody] RelatorioFiltro105DTO filtro)
        {
            // 1. Busca os dados usando a lógica que já temos na Controller
            var actionResult = await GetDadosRelatorio105(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var lista105 = okResult?.Value as List<Relatorio105DTO>;

            if (lista105 == null || !lista105.Any())
                return BadRequest("Nenhum produto sem giro encontrado para os critérios selecionados.");

            // 2. Chama o serviço do QuestPDF
            var service = new RelatorioExportService();
            var pdfBytes = service.GerarPdfRelatorio105(lista105, filtro.DiasSemGiro);

            return File(pdfBytes, "application/pdf", "RelatorioProdutosSemGiro.pdf");
        }
        #endregion

        #region RELATÓRIO 106 - RANKING DE VENDAS (MAIS/MENOS VENDIDOS)

        [HttpPost("106/dados")]
        public async Task<ActionResult<List<Relatorio106DTO>>> GetDadosRelatorio106([FromBody] RelatorioFiltro106DTO filtro)
        {
            var query = _context.ProdutosMovimento
                .Include(pm => pm.Produto).ThenInclude(p => p.Familia).ThenInclude(f => f.Categoria)
                .Include(pm => pm.ProdutoEmbalagem)
                .Include(pm => pm.Movimento)
                .AsNoTracking()
                .AsQueryable();

            if (filtro.DataInicio.HasValue)
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento <= filtro.DataFim.Value);

            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(pm => pm.Produto.Familia != null && filtro.CategoriasIds.Contains(pm.Produto.Familia.CategoriaId));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(pm => filtro.FamiliasIds.Contains(pm.Produto.FamiliaId));

            if (filtro.MarcasIds != null && filtro.MarcasIds.Any())
                query = query.Where(pm => pm.Produto.Familia != null && pm.Produto.Familia.MarcaId != null && filtro.MarcasIds.Contains(pm.Produto.Familia.MarcaId.Value));

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(pm => filtro.ProdutosIds.Contains(pm.ProdutoId));

            var dadosAgrupados = await query
                .Select(pm => new
                {
                    ProdutoId = pm.ProdutoId,
                    NomeProduto = pm.Produto.NomeProduto,
                    Categoria = pm.Produto.Familia != null && pm.Produto.Familia.Categoria != null ? pm.Produto.Familia.Categoria.DescricaoCategoria : "Sem Categoria",
                    Quantidade = Math.Abs(pm.Quantidade),
                    ValorTotal = Math.Abs(pm.Quantidade) * (pm.ProdutoEmbalagem != null ? pm.ProdutoEmbalagem.Preco : 0)
                })
                .GroupBy(x => new { x.ProdutoId, x.NomeProduto, x.Categoria })
                .Select(g => new Relatorio106DTO
                {
                    IdProduto = g.Key.ProdutoId,
                    NomeProduto = g.Key.NomeProduto,
                    Categoria = g.Key.Categoria,
                    QuantidadeVendida = g.Sum(x => x.Quantidade),
                    Faturamento = g.Sum(x => x.ValorTotal)
                })
                .Where(x => x.QuantidadeVendida > 0) // Pega só o que vendeu alguma coisa
                .ToListAsync();

            // A mágica que você sugeriu acontece aqui:
            if (filtro.TipoOrdem == "MENOS_VENDIDOS")
                dadosAgrupados = dadosAgrupados.OrderBy(x => x.QuantidadeVendida).Take(filtro.QuantidadeRegistros).ToList();
            else
                dadosAgrupados = dadosAgrupados.OrderByDescending(x => x.QuantidadeVendida).Take(filtro.QuantidadeRegistros).ToList();

            return Ok(dadosAgrupados);
        }

        [HttpPost("106/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio106([FromBody] RelatorioFiltro106DTO filtro)
        {
            var actionResult = await GetDadosRelatorio106(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var lista106 = okResult?.Value as List<Relatorio106DTO>;

            if (lista106 == null || !lista106.Any())
                return BadRequest("Nenhum dado encontrado para o período selecionado.");

            var service = new RelatorioExportService();
            // Muda o título do PDF dependendo da opção escolhida
            var titulo = filtro.TipoOrdem == "MENOS_VENDIDOS" ? "PRODUTOS MENOS VENDIDOS" : "PRODUTOS MAIS VENDIDOS";
            var pdfBytes = service.GerarPdfRelatorio106(lista106, titulo);

            return File(pdfBytes, "application/pdf", $"RankingVendas_{DateTime.Now:ddMMyyyy}.pdf");
        }

        #endregion

        #region RELATÓRIO 107 - HISTÓRICO DE ALTERAÇÃO DE PREÇOS

        [HttpPost("107/dados")]
        public async Task<ActionResult<List<Relatorio107DTO>>> GetDadosRelatorio107([FromBody] RelatorioFiltroProdutosDTO filtro)
        {
            // AGORA SIM, LENDO DA TABELA CORRETA
            var query = _context.HistoricosPreco
                .Include(h => h.Produto).ThenInclude(p => p.Familia).ThenInclude(f => f.Categoria)
                .Include(h => h.Produto).ThenInclude(p => p.Familia).ThenInclude(f => f.Marca)
                .AsNoTracking()
                .AsQueryable();

            if (filtro.DataInicio.HasValue)
                query = query.Where(h => h.DataAlteracao >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(h => h.DataAlteracao <= filtro.DataFim.Value);

            // Filtros de Produto/Categoria
            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(h => h.Produto.Familia != null && filtro.CategoriasIds.Contains(h.Produto.Familia.CategoriaId));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(h => filtro.FamiliasIds.Contains(h.Produto.FamiliaId));

            if (filtro.MarcasIds != null && filtro.MarcasIds.Any())
                query = query.Where(h => h.Produto.Familia != null && h.Produto.Familia.MarcaId != null && filtro.MarcasIds.Contains(h.Produto.Familia.MarcaId.Value));

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(h => filtro.ProdutosIds.Contains(h.ProdutoId));

            var lista = await query
                .OrderByDescending(h => h.DataAlteracao)
                .Select(h => new Relatorio107DTO
                {
                    IdProduto = h.ProdutoId,
                    NomeProduto = h.Produto.NomeProduto ?? "Sem Nome",
                    Categoria = h.Produto.Familia != null && h.Produto.Familia.Categoria != null ? h.Produto.Familia.Categoria.DescricaoCategoria : "Sem Categoria",
                    DataAlteracao = h.DataAlteracao,
                    PrecoAnterior = h.PrecoAntigo,
                    PrecoNovo = h.PrecoNovo,
                    Usuario = string.IsNullOrEmpty(h.Usuario) ? "Sistema" : h.Usuario
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost("107/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio107([FromBody] RelatorioFiltroProdutosDTO filtro)
        {
            var actionResult = await GetDadosRelatorio107(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var lista107 = okResult?.Value as List<Relatorio107DTO>;

            if (lista107 == null || !lista107.Any())
                return BadRequest("Nenhuma alteração de preço encontrada para os filtros aplicados.");

            var service = new RelatorioExportService();
            var pdfBytes = service.GerarPdfRelatorio107(lista107);

            return File(pdfBytes, "application/pdf", $"HistoricoPrecos_{DateTime.Now:ddMMyyyy}.pdf");
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
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
            {
                var dataInicio = filtro.DataInicio.Value.Date;
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento >= dataInicio);
            }

            if (filtro.DataFim.HasValue)
            {
                var dataFim = filtro.DataFim.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento <= dataFim);
            }

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

            // Filtros de Data (Blindados)
            if (filtro.DataInicio.HasValue)
            {
                var dataInicio = filtro.DataInicio.Value.Date;
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento >= dataInicio);
            }

            if (filtro.DataFim.HasValue)
            {
                var dataFim = filtro.DataFim.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento <= dataFim);
            }

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

            // Filtros de Data (Blindados)
            if (filtro.DataInicio.HasValue)
            {
                var dataInicio = filtro.DataInicio.Value.Date;
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento >= dataInicio);
            }

            if (filtro.DataFim.HasValue)
            {
                var dataFim = filtro.DataFim.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento <= dataFim);
            }

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
            {
                var dataInicio = filtro.DataInicio.Value.Date;
                query = query.Where(h => h.DataAlteracao >= dataInicio);
            }

            if (filtro.DataFim.HasValue)
            {
                var dataFim = filtro.DataFim.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(h => h.DataAlteracao <= dataFim);
            }

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

        #region RELATÓRIO 201 - POSIÇÃO ATUAL DE ESTOQUE

        [HttpPost("201/dados")]
        public async Task<ActionResult<List<Relatorio201DTO>>> GetDadosRelatorio201([FromBody] RelatorioFiltro201DTO filtro)
        {
            var query = _context.Produtos
                .Include(p => p.Familia).ThenInclude(f => f.Categoria)
                .Include(p => p.Familia).ThenInclude(f => f.Marca)
                .AsNoTracking()
                .AsQueryable();

            // Filtros Padrão (Herdados)
            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(p => p.Familia != null && filtro.CategoriasIds.Contains(p.Familia.CategoriaId));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(p => filtro.FamiliasIds.Contains(p.FamiliaId));

            if (filtro.MarcasIds != null && filtro.MarcasIds.Any())
                query = query.Where(p => p.Familia != null && p.Familia.MarcaId != null && filtro.MarcasIds.Contains(p.Familia.MarcaId.Value));

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(p => filtro.ProdutosIds.Contains(p.IdProduto));

            if (filtro.Ativo.HasValue)
                query = query.Where(p => p.Ativo == filtro.Ativo.Value);

            // Filtro Específico de Saldo de Estoque
            if (filtro.FiltroSaldo == "COM_ESTOQUE")
                query = query.Where(p => p.EstoqueAtual > 0);
            else if (filtro.FiltroSaldo == "SEM_ESTOQUE")
                query = query.Where(p => p.EstoqueAtual == 0);
            else if (filtro.FiltroSaldo == "NEGATIVO")
                query = query.Where(p => p.EstoqueAtual < 0);

            var lista = await query
                .OrderBy(p => p.NomeProduto)
                .Select(p => new Relatorio201DTO
                {
                    IdProduto = p.IdProduto,
                    NomeProduto = p.NomeProduto ?? "Sem Nome",
                    Categoria = p.Familia != null && p.Familia.Categoria != null ? p.Familia.Categoria.DescricaoCategoria : "Sem Categoria",
                    Marca = p.Familia != null && p.Familia.Marca != null ? p.Familia.Marca.NomeMarca : "Sem Marca",
                    EstoqueAtual = p.EstoqueAtual,
                    CustoMedio = p.CustoMedio
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost("201/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio201([FromBody] RelatorioFiltro201DTO filtro)
        {
            var actionResult = await GetDadosRelatorio201(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var lista201 = okResult?.Value as List<Relatorio201DTO>;

            if (lista201 == null || !lista201.Any())
                return BadRequest("Nenhum produto encontrado para a posição de estoque informada.");

            var service = new RelatorioExportService();
            var pdfBytes = service.GerarPdfRelatorio201(lista201);

            return File(pdfBytes, "application/pdf", $"PosicaoEstoque_{DateTime.Now:ddMMyyyy}.pdf");
        }

        #endregion

        #region RELATÓRIO 202 - LOTES E VALIDADES

        [HttpPost("202/dados")]
        public async Task<ActionResult<List<Relatorio202DTO>>> GetDadosRelatorio202([FromBody] RelatorioFiltro202DTO filtro)
        {
            var hoje = DateTime.Now.Date;

            // Busca nas validades que ainda estão marcadas como EmEstoque
            var query = _context.Validades
                .Include(v => v.Produto).ThenInclude(p => p.Familia).ThenInclude(f => f.Categoria)
                .Where(v => v.EmEstoque == true)
                .AsNoTracking()
                .AsQueryable();

            // Filtros Padrão de Produto (Herdados)
            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(v => v.Produto.Familia != null && filtro.CategoriasIds.Contains(v.Produto.Familia.CategoriaId));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(v => filtro.FamiliasIds.Contains(v.Produto.FamiliaId));

            if (filtro.MarcasIds != null && filtro.MarcasIds.Any())
                query = query.Where(v => v.Produto.Familia != null && v.Produto.Familia.MarcaId != null && filtro.MarcasIds.Contains(v.Produto.Familia.MarcaId.Value));

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(v => filtro.ProdutosIds.Contains(v.ProdutoId));

            // Filtro Específico de Risco de Vencimento
            if (filtro.StatusValidade == "VENCIDOS")
                query = query.Where(v => v.DataValidade.Date < hoje);
            else if (filtro.StatusValidade == "VENCENDO_30")
                query = query.Where(v => v.DataValidade.Date >= hoje && v.DataValidade.Date <= hoje.AddDays(30));
            else if (filtro.StatusValidade == "NO_PRAZO")
                query = query.Where(v => v.DataValidade.Date > hoje.AddDays(30));

            var lista = await query
                .OrderBy(v => v.DataValidade) // Ordena do risco mais alto (vencido) para o mais baixo
                .Select(v => new Relatorio202DTO
                {
                    IdProduto = v.ProdutoId,
                    NomeProduto = v.Produto.NomeProduto ?? "Sem Nome",
                    Categoria = v.Produto.Familia != null && v.Produto.Familia.Categoria != null ? v.Produto.Familia.Categoria.DescricaoCategoria : "Sem Categoria",
                    DataValidade = v.DataValidade
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost("202/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio202([FromBody] RelatorioFiltro202DTO filtro)
        {
            var actionResult = await GetDadosRelatorio202(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var lista202 = okResult?.Value as List<Relatorio202DTO>;

            if (lista202 == null || !lista202.Any())
                return BadRequest("Nenhuma validade encontrada para os filtros aplicados.");

            var service = new RelatorioExportService();
            var pdfBytes = service.GerarPdfRelatorio202(lista202);

            return File(pdfBytes, "application/pdf", $"LotesValidades_{DateTime.Now:ddMMyyyy}.pdf");
        }

        #endregion

        #region RELATÓRIO 203 - MOVIMENTAÇÃO DE ESTOQUE GERAL

        [HttpPost("203/dados")]
        public async Task<ActionResult<List<Relatorio203DTO>>> GetDadosRelatorio203([FromBody] RelatorioFiltro203DTO filtro)
        {
            var query = _context.ProdutosMovimento
                .Include(pm => pm.Movimento).ThenInclude(m => m.TipoMovimento)
                .Include(pm => pm.Movimento).ThenInclude(m => m.Pessoa)
                .Include(pm => pm.Produto).ThenInclude(p => p.Familia).ThenInclude(f => f.Categoria)
                .Include(pm => pm.ProdutoEmbalagem)
                .AsNoTracking()
                .AsQueryable();

            // Filtros de Data
            if (filtro.DataInicio.HasValue)
            {
                var dataInicio = filtro.DataInicio.Value.Date;
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento >= dataInicio);
            }

            if (filtro.DataFim.HasValue)
            {
                var dataFim = filtro.DataFim.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento <= dataFim);
            }

            // Filtros Padrão de Produto
            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(pm => pm.Produto.Familia != null && filtro.CategoriasIds.Contains(pm.Produto.Familia.CategoriaId));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(pm => filtro.FamiliasIds.Contains(pm.Produto.FamiliaId));

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(pm => filtro.ProdutosIds.Contains(pm.ProdutoId));

            // Filtro Específico (Entradas x Saídas)
            if (filtro.TipoOperacao == "ENTRADAS")
                query = query.Where(pm => pm.Quantidade > 0);
            else if (filtro.TipoOperacao == "SAIDAS")
                query = query.Where(pm => pm.Quantidade < 0);

            var lista = await query
                .OrderByDescending(pm => pm.Movimento.DataMovimento)
                .Select(pm => new Relatorio203DTO
                {
                    IdMovimento = pm.MovimentoId,
                    DataMovimento = pm.Movimento != null ? pm.Movimento.DataMovimento : DateTime.MinValue,
                    TipoMovimento = pm.Movimento != null && pm.Movimento.TipoMovimento != null ? pm.Movimento.TipoMovimento.DescricaoTipoMovimento : "Geral",
                    Pessoa = pm.Movimento != null && pm.Movimento.Pessoa != null ? pm.Movimento.Pessoa.NomeRazao : "Consumidor Final / Não Informado",
                    IdProduto = pm.ProdutoId,
                    NomeProduto = pm.Produto.NomeProduto ?? "Sem Nome",
                    Quantidade = pm.Quantidade,
                    // Assume o preço da embalagem ou o custo médio como referência de valor
                    ValorUnitario = pm.ProdutoEmbalagem != null ? pm.ProdutoEmbalagem.Preco : pm.Produto.CustoMedio
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost("203/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio203([FromBody] RelatorioFiltro203DTO filtro)
        {
            var actionResult = await GetDadosRelatorio203(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var lista203 = okResult?.Value as List<Relatorio203DTO>;

            if (lista203 == null || !lista203.Any())
                return BadRequest("Nenhuma movimentação encontrada para o período e filtros aplicados.");

            var service = new RelatorioExportService();
            var pdfBytes = service.ExportarPdfRelatorio203(lista203);

            return File(pdfBytes, "application/pdf", $"MovimentacaoEstoque_{DateTime.Now:ddMMyyyy}.pdf");
        }

        #endregion

        #region RELATÓRIO 204 - COBERTURA E SUGESTÃO DE COMPRAS

        [HttpPost("204/dados")]
        public async Task<ActionResult<List<Relatorio204DTO>>> GetDadosRelatorio204([FromBody] RelatorioFiltro204DTO filtro)
        {
            var dataCorte = DateTime.Now.Date.AddDays(-filtro.DiasAnaliseGiro);

            var query = _context.Produtos
                .Include(p => p.Familia).ThenInclude(f => f.Categoria)
                .Include(p => p.Familia).ThenInclude(f => f.Marca)
                .AsNoTracking()
                .AsQueryable();

            // Filtros Padrão de Produto
            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(p => p.Familia != null && filtro.CategoriasIds.Contains(p.Familia.CategoriaId));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(p => filtro.FamiliasIds.Contains(p.FamiliaId));

            if (filtro.MarcasIds != null && filtro.MarcasIds.Any())
                query = query.Where(p => p.Familia != null && p.Familia.MarcaId != null && filtro.MarcasIds.Contains(p.Familia.MarcaId.Value));

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(p => filtro.ProdutosIds.Contains(p.IdProduto));

            // Apenas produtos ativos precisam de reposição
            query = query.Where(p => p.Ativo == true);

            // Seleciona o produto e soma as saídas dele no período
            var listaCrua = await query.Select(p => new
            {
                Produto = p,
                // Pega as quantidades < 0 (Saídas/Vendas) que aconteceram após a Data de Corte
                SaidasPeriodo = _context.ProdutosMovimento
                    .Where(pm => pm.ProdutoId == p.IdProduto && pm.Quantidade < 0 && pm.Movimento != null && pm.Movimento.DataMovimento >= dataCorte)
                    .Sum(pm => (decimal?)Math.Abs(pm.Quantidade)) ?? 0
            }).ToListAsync();

            var listaProcessada = listaCrua.Select(x => new Relatorio204DTO
            {
                IdProduto = x.Produto.IdProduto,
                NomeProduto = x.Produto.NomeProduto ?? "Sem Nome",
                Categoria = x.Produto.Familia != null && x.Produto.Familia.Categoria != null ? x.Produto.Familia.Categoria.DescricaoCategoria : "Sem Categoria",
                EstoqueAtual = x.Produto.EstoqueAtual,
                VendaTotalPeriodo = x.SaidasPeriodo,
                DiasAnalise = filtro.DiasAnaliseGiro,
                DiasDesejados = filtro.DiasCoberturaDesejada
            })
            // Mostra apenas itens que têm sugestão de compra OU que o estoque está abaixo de zero
            .Where(x => x.SugestaoCompra > 0 || x.EstoqueAtual <= 0)
            .OrderByDescending(x => x.SugestaoCompra) // Os itens mais urgentes aparecem primeiro
            .ToList();

            return Ok(listaProcessada);
        }

        [HttpPost("204/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio204([FromBody] RelatorioFiltro204DTO filtro)
        {
            var actionResult = await GetDadosRelatorio204(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var lista204 = okResult?.Value as List<Relatorio204DTO>;

            if (lista204 == null || !lista204.Any())
                return BadRequest("Seu estoque está saudável! Nenhuma necessidade de compra detectada.");

            var service = new RelatorioExportService();
            var pdfBytes = service.ExportarPdfRelatorio204(lista204, filtro.DiasCoberturaDesejada);

            return File(pdfBytes, "application/pdf", $"SugestaoCompras_{DateTime.Now:ddMMyyyy}.pdf");
        }

        #endregion

        #region RELATÓRIO 205 - INVENTÁRIO E ESTOQUE MÍNIMO

        [HttpPost("205/dados")]
        public async Task<ActionResult<List<Relatorio205DTO>>> GetDadosRelatorio205([FromBody] RelatorioFiltro205DTO filtro)
        {
            var query = _context.Produtos
                .Include(p => p.Familia).ThenInclude(f => f.Categoria)
                .Where(p => p.Ativo)
                .AsNoTracking()
                .AsQueryable();

            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any()) query = query.Where(p => p.Familia != null && filtro.CategoriasIds.Contains(p.Familia.CategoriaId));
            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any()) query = query.Where(p => filtro.FamiliasIds.Contains(p.FamiliaId));
            if (filtro.MarcasIds != null && filtro.MarcasIds.Any()) query = query.Where(p => p.Familia != null && p.Familia.MarcaId != null && filtro.MarcasIds.Contains(p.Familia.MarcaId.Value));
            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any()) query = query.Where(p => filtro.ProdutosIds.Contains(p.IdProduto));

            // Mágica da Fusão: Só filtra pelo mínimo se o switch estiver ligado
            if (filtro.ApenasAbaixoDoMinimo)
            {
                var produtosAbaixoDoMinimo = _context.Set<EstoqueConfig>()
                    .Where(e => e.EstoqueMinimo > 0 && e.Produto.EstoqueAtual < e.EstoqueMinimo)
                    .Select(e => e.ProdutoId);

                query = query.Where(p => produtosAbaixoDoMinimo.Contains(p.IdProduto));
            }

            var lista = await query
                .OrderBy(p => p.Familia != null && p.Familia.Categoria != null ? p.Familia.Categoria.DescricaoCategoria : "")
                .ThenBy(p => p.NomeProduto)
                .Select(p => new Relatorio205DTO
                {
                    IdProduto = p.IdProduto,
                    NomeProduto = p.NomeProduto ?? "",
                    Categoria = p.Familia != null && p.Familia.Categoria != null ? p.Familia.Categoria.DescricaoCategoria : "",
                    EstoqueAtual = p.EstoqueAtual,
                    EstoqueMinimo = _context.Set<EstoqueConfig>().Where(e => e.ProdutoId == p.IdProduto).Select(e => e.EstoqueMinimo).FirstOrDefault()
                }).ToListAsync();

            return Ok(lista);
        }

        [HttpPost("205/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio205([FromBody] RelatorioFiltro205DTO filtro)
        {
            var dados = (await GetDadosRelatorio205(filtro)).Result as OkObjectResult;
            var lista = dados?.Value as List<Relatorio205DTO>;
            if (lista == null || !lista.Any()) return BadRequest("Nenhum produto encontrado para a ficha de contagem.");

            var pdfBytes = new RelatorioExportService().ExportarPdfRelatorio205(lista, filtro.OcultarEstoqueSistema);
            return File(pdfBytes, "application/pdf", $"Inventario_{DateTime.Now:ddMMyyyy}.pdf");
        }
        #endregion

        #region RELATÓRIO 206 - VALORIZAÇÃO DE ESTOQUE E PROJEÇÃO

        [HttpPost("206/dados")]
        public async Task<ActionResult<List<Relatorio206DTO>>> GetDadosRelatorio206([FromBody] RelatorioFiltro206DTO filtro)
        {
            var query = _context.Produtos
                .Include(p => p.Familia).ThenInclude(f => f.Categoria)
                .AsNoTracking()
                .AsQueryable();

            // Filtros Padrão
            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any())
                query = query.Where(p => p.Familia != null && filtro.CategoriasIds.Contains(p.Familia.CategoriaId));

            if (filtro.FamiliasIds != null && filtro.FamiliasIds.Any())
                query = query.Where(p => filtro.FamiliasIds.Contains(p.FamiliaId));

            if (filtro.MarcasIds != null && filtro.MarcasIds.Any())
                query = query.Where(p => p.Familia != null && p.Familia.MarcaId != null && filtro.MarcasIds.Contains(p.Familia.MarcaId.Value));

            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(p => filtro.ProdutosIds.Contains(p.IdProduto));

            // Apenas produtos ativos
            var ativo = filtro.Ativo ?? true;
            query = query.Where(p => p.Ativo == ativo);

            if (filtro.ApenasComEstoque)
                query = query.Where(p => p.EstoqueAtual > 0);

            var lista = await query
                .OrderBy(p => p.NomeProduto)
                .Select(p => new Relatorio206DTO
                {
                    IdProduto = p.IdProduto,
                    NomeProduto = p.NomeProduto ?? "Sem Nome",
                    Categoria = p.Familia != null && p.Familia.Categoria != null ? p.Familia.Categoria.DescricaoCategoria : "Sem Categoria",
                    EstoqueAtual = p.EstoqueAtual,
                    CustoMedio = p.CustoMedio,
                    // Puxa o preço de venda da primeira embalagem vinculada ao produto
                    PrecoVenda = _context.ProdutosEmbalagem.Where(pe => pe.ProdutoId == p.IdProduto).Select(pe => pe.Preco).FirstOrDefault()
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost("206/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio206([FromBody] RelatorioFiltro206DTO filtro)
        {
            var actionResult = await GetDadosRelatorio206(filtro);
            var okResult = actionResult.Result as OkObjectResult;
            var lista206 = okResult?.Value as List<Relatorio206DTO>;

            if (lista206 == null || !lista206.Any())
                return BadRequest("Nenhum produto com estoque encontrado para valorização.");

            var service = new RelatorioExportService();
            var pdfBytes = service.ExportarPdfRelatorio206(lista206);

            return File(pdfBytes, "application/pdf", $"ValorizacaoEstoque_{DateTime.Now:ddMMyyyy}.pdf");
        }

        #endregion

        #region RELATÓRIO 207 - GIRO DE ESTOQUE (VELOCIDADE)
        [HttpPost("207/dados")]
        public async Task<ActionResult<List<Relatorio207DTO>>> GetDadosRelatorio207([FromBody] RelatorioFiltro207DTO filtro)
        {
            var dataInicioBase = filtro.DataInicio ?? DateTime.Now.AddDays(-30);
            var dataFimBase = filtro.DataFim ?? DateTime.Now;

            var dataInicio = dataInicioBase.Date;
            var dataFim = dataFimBase.Date.AddDays(1).AddSeconds(-1);

            var dias = (dataFimBase.Date - dataInicioBase.Date).Days;
            if (dias <= 0) dias = 1;

            var query = _context.Produtos.Include(p => p.Familia).ThenInclude(f => f.Categoria).AsNoTracking().AsQueryable();

            if (filtro.CategoriasIds != null && filtro.CategoriasIds.Any()) query = query.Where(p => p.Familia != null && filtro.CategoriasIds.Contains(p.Familia.CategoriaId));
            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any()) query = query.Where(p => filtro.ProdutosIds.Contains(p.IdProduto));

            var listaCrua = await query.Select(p => new {
                Produto = p,
                Saidas = _context.ProdutosMovimento.Where(pm => pm.ProdutoId == p.IdProduto && pm.Quantidade < 0 && pm.Movimento != null && pm.Movimento.DataMovimento >= dataInicio && pm.Movimento.DataMovimento <= dataFim).Sum(pm => (decimal?)Math.Abs(pm.Quantidade)) ?? 0
            }).ToListAsync();

            var lista = listaCrua.Select(x => new Relatorio207DTO
            {
                IdProduto = x.Produto.IdProduto,
                NomeProduto = x.Produto.NomeProduto ?? "",
                Categoria = x.Produto.Familia?.Categoria?.DescricaoCategoria ?? "",
                EstoqueAtual = x.Produto.EstoqueAtual,
                TotalSaidas = x.Saidas,
                DiasAnalisados = dias
            }).Where(x => x.TotalSaidas > 0).OrderByDescending(x => x.TotalSaidas).ToList();

            return Ok(lista);
        }

        [HttpPost("207/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio207([FromBody] RelatorioFiltro207DTO filtro)
        {
            var dados = (await GetDadosRelatorio207(filtro)).Result as OkObjectResult;
            var lista = dados?.Value as List<Relatorio207DTO>;
            if (lista == null || !lista.Any()) return BadRequest("Sem dados de giro.");
            return File(new RelatorioExportService().ExportarPdfRelatorio207(lista), "application/pdf", "GiroEstoque.pdf");
        }
        #endregion

        #region RELATÓRIO 208 - DIVERGÊNCIA DE INVENTÁRIO
        [HttpPost("208/dados")]
        public async Task<ActionResult<List<Relatorio208DTO>>> GetDadosRelatorio208([FromBody] RelatorioFiltro208DTO filtro)
        {
            var query = _context.ProdutosMovimento.Include(pm => pm.Produto).Include(pm => pm.Movimento).AsNoTracking().AsQueryable();

            if (filtro.DataInicio.HasValue)
            {
                var dataInicio = filtro.DataInicio.Value.Date;
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento >= dataInicio);
            }

            if (filtro.DataFim.HasValue)
            {
                var dataFim = filtro.DataFim.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento <= dataFim);
            }

            if (filtro.IdTipoMovimentoAjuste.HasValue) query = query.Where(pm => pm.Movimento != null && pm.Movimento.TipoMovimentoId == filtro.IdTipoMovimentoAjuste);
            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any()) query = query.Where(pm => filtro.ProdutosIds.Contains(pm.ProdutoId));

            var lista = await query.Select(pm => new Relatorio208DTO
            {
                IdProduto = pm.ProdutoId,
                NomeProduto = pm.Produto.NomeProduto ?? "",
                DataAjuste = pm.Movimento != null ? pm.Movimento.DataMovimento : DateTime.MinValue,
                QuantidadeAjustada = pm.Quantidade,
                CustoUnitario = pm.Produto.CustoMedio,
                Observacao = pm.Movimento != null && pm.Movimento.Observacao != null ? pm.Movimento.Observacao : ""
            }).OrderByDescending(x => x.DataAjuste).ToListAsync();

            return Ok(lista);
        }

        [HttpPost("208/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio208([FromBody] RelatorioFiltro208DTO filtro)
        {
            var dados = (await GetDadosRelatorio208(filtro)).Result as OkObjectResult;
            var lista = dados?.Value as List<Relatorio208DTO>;
            if (lista == null || !lista.Any()) return BadRequest("Nenhuma divergência no período.");
            return File(new RelatorioExportService().ExportarPdfRelatorio208(lista), "application/pdf", "Divergencias.pdf");
        }
        #endregion

        #region RELATÓRIO 301 - HISTÓRICO ANALÍTICO DE MOVIMENTAÇÕES

        [HttpPost("301/dados")]
        public async Task<ActionResult<List<Relatorio301DTO>>> GetDadosRelatorio301([FromBody] RelatorioFiltro301DTO filtro)
        {
            var query = _context.ProdutosMovimento
                .Include(pm => pm.Movimento).ThenInclude(m => m.TipoMovimento)
                .Include(pm => pm.Movimento).ThenInclude(m => m.Pessoa)
                .Include(pm => pm.Produto)
                .AsNoTracking()
                .AsQueryable();

            // 1. Filtro Blindado de Datas
            if (filtro.DataInicio.HasValue)
            {
                var dataInicio = filtro.DataInicio.Value.Date;
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento >= dataInicio);
            }

            if (filtro.DataFim.HasValue)
            {
                var dataFim = filtro.DataFim.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(pm => pm.Movimento != null && pm.Movimento.DataMovimento <= dataFim);
            }

            // 2. Filtro de Tipos de Movimento (Entrada, Saída, Ajuste, etc)
            if (filtro.TiposMovimentoIds != null && filtro.TiposMovimentoIds.Any())
                query = query.Where(pm => pm.Movimento != null && filtro.TiposMovimentoIds.Contains(pm.Movimento.TipoMovimentoId));

            // 3. Filtro de Pessoas / Usuários
            if (filtro.PessoasIds != null && filtro.PessoasIds.Any())
                query = query.Where(pm => pm.Movimento != null && filtro.PessoasIds.Contains(pm.Movimento.PessoaId));

            // 4. Filtro Específico de Produtos
            if (filtro.ProdutosIds != null && filtro.ProdutosIds.Any())
                query = query.Where(pm => filtro.ProdutosIds.Contains(pm.ProdutoId));

            var lista = await query
                .OrderByDescending(pm => pm.Movimento != null ? pm.Movimento.DataMovimento : DateTime.MinValue)
                .Select(pm => new Relatorio301DTO
                {
                    IdMovimento = pm.MovimentoId,
                    DataMovimento = pm.Movimento != null ? pm.Movimento.DataMovimento : DateTime.MinValue,
                    TipoMovimento = pm.Movimento != null && pm.Movimento.TipoMovimento != null ? pm.Movimento.TipoMovimento.DescricaoTipoMovimento : "Não Informado",
                    Pessoa = pm.Movimento != null && pm.Movimento.Pessoa != null ? pm.Movimento.Pessoa.NomeRazao : "Sistema/Geral",
                    Produto = pm.Produto != null ? pm.Produto.NomeProduto ?? "Sem Nome" : "Sem Nome",
                    Quantidade = pm.Quantidade,
                    ValorUnitario = pm.Produto != null ? pm.Produto.CustoMedio : 0, // Pode trocar para Preço se preferir
                    Observacao = pm.Movimento != null && pm.Movimento.Observacao != null ? pm.Movimento.Observacao : ""
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost("301/exportar/pdf")]
        public async Task<IActionResult> ExportarPdfRelatorio301([FromBody] RelatorioFiltro301DTO filtro)
        {
            var dados = (await GetDadosRelatorio301(filtro)).Result as OkObjectResult;
            var lista = dados?.Value as List<Relatorio301DTO>;

            if (lista == null || !lista.Any())
                return BadRequest("Nenhuma movimentação encontrada para o período e filtros selecionados.");

            var service = new RelatorioExportService();
            var pdfBytes = service.ExportarPdfRelatorio301(lista);

            return File(pdfBytes, "application/pdf", $"HistoricoMovimentacoes_{DateTime.Now:ddMMyyyy}.pdf");
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
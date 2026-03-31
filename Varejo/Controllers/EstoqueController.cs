using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Varejo.Interfaces;
using Varejo.ViewModels;
using Varejo.Data;
using Microsoft.EntityFrameworkCore;

namespace Varejo.Controllers
{
    public class EstoqueController : Controller
    {
        private readonly IEstoqueRepository _estoqueRepo;
        private readonly VarejoDbContext _context;

        public EstoqueController(IEstoqueRepository estoqueRepo, VarejoDbContext context)
        {
            _estoqueRepo = estoqueRepo;
            _context = context;
        }

        // Listagem com Filtros
        public async Task<IActionResult> Index(EstoqueFiltroViewModel filtro)
        {
            // Carregar combos para os filtros da Sidebar
            ViewBag.Marcas = new SelectList(await _context.Marcas.OrderBy(m => m.NomeMarca).ToListAsync(), "IdMarca", "NomeMarca", filtro.MarcaId);
            ViewBag.Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.DescricaoCategoria).ToListAsync(), "IdCategoria", "DescricaoCategoria", filtro.CategoriaId);

            var viewModel = await _estoqueRepo.ObterEstoqueGeralAsync(filtro);
            return View(viewModel);
        }

        // Endpoint para o Snapshot manual (Botão de manutenção)
        [HttpPost]
        public async Task<IActionResult> GerarSnapshot()
        {
            int total = await _estoqueRepo.GerarSnapshotDiarioAsync();
            TempData["Success"] = $"Snapshot gerado com sucesso para {total} produtos!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Familia)
                .FirstOrDefaultAsync(p => p.IdProduto == id);

            if (produto == null) return NotFound();

            // Query unindo Histórico com Movimento e TipoMovimento
            var historicoVm = await _context.HistoricosProduto
                .Where(h => h.ProdutoId == id)
                .Join(_context.Movimentos,
                    h => h.MovimentoId,
                    m => m.IdMovimento,
                    (h, m) => new { h, m })
                .Select(join => new HistoricoProdutoViewModel
                {
                    Id = join.h.Id,
                    Data = join.h.Data,
                    // Buscamos a descrição lá do TipoMovimento vinculado ao cabeçalho
                    TipoMovimento = join.m.TipoMovimento.DescricaoTipoMovimento,
                    EstoqueAntes = join.h.EstoqueAntes,
                    QuantidadeMovimento = join.h.QuantidadeMovimento,
                    EstoqueDepois = join.h.EstoqueDepois,
                    Observacao = join.h.Observacao
                })
                .OrderByDescending(x => x.Data)
                .ThenByDescending(x => x.Id)
                .ToListAsync();

            ViewBag.Produto = produto;
            return View(historicoVm);
        }

        public async Task<IActionResult> PrintDetails(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Familia)
                .FirstOrDefaultAsync(p => p.IdProduto == id);

            if (produto == null) return NotFound();

            var historicoVm = await _context.HistoricosProduto
                .Where(h => h.ProdutoId == id)
                .Join(_context.Movimentos,
                    h => h.MovimentoId,
                    m => m.IdMovimento,
                    (h, m) => new { h, m })
                .Select(join => new HistoricoProdutoViewModel
                {
                    Data = join.h.Data,
                    TipoMovimento = join.m.TipoMovimento.DescricaoTipoMovimento,
                    EstoqueAntes = join.h.EstoqueAntes,
                    QuantidadeMovimento = join.h.QuantidadeMovimento,
                    EstoqueDepois = join.h.EstoqueDepois,
                    Observacao = join.h.Observacao
                })
                .OrderByDescending(x => x.Data)
                .ToListAsync();

            ViewBag.Produto = produto;
            // Retorna a View de impressão que vamos criar abaixo
            return View("PrintDetails", historicoVm);
        }

    }
}
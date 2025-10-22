using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Repositories;
using Varejo.ViewModels;
using Varejo.Models;
using Varejo.Interfaces;

namespace Varejo.Controllers
{
    public class MovimentoController : Controller
    {
        private readonly VarejoDbContext _context;
        private readonly IProdutoMovimentoRepository _produtoMovimentoRepository;
        private readonly IProdutoRepository _produtoRepository;

        public MovimentoController(VarejoDbContext context, IProdutoMovimentoRepository produtoMovimentoRepository, IProdutoRepository produtoRepository)
        {
            _context = context;
            _produtoMovimentoRepository = produtoMovimentoRepository;
            _produtoRepository = produtoRepository;
        }

        // LISTAR TODOS
        public async Task<IActionResult> Index()
        {
            var movimentos = await _context.Movimentos
                .Include(m => m.Pessoa)
                .Include(m => m.TipoMovimento)
                .Include(m => m.ProdutosMovimento)
                .ThenInclude(pm => pm.Produto)
                .ToListAsync();

            return View(movimentos);
        }

        // DETALHES
        public async Task<IActionResult> Details(int id)
        {
            var movimento = await _context.Movimentos
                .Include(m => m.Pessoa)
                .Include(m => m.TipoMovimento)
                .Include(m => m.ProdutosMovimento)
                    .ThenInclude(pm => pm.Produto)
                .Include(m => m.ProdutosMovimento)
                    .ThenInclude(pm => pm.ProdutoEmbalagem)
                        .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(m => m.IdMovimento == id);

            if (movimento == null) return NotFound();

            return View(movimento);
        }


        [HttpGet]
        public async Task<IActionResult> SearchProduto([FromQuery] string query)
        {
            Console.WriteLine($"[SearchProduto] Query recebida: '{query}'"); // <- log simples

            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<ProdutoViewModel>());

            var produtos = await _produtoRepository.GetByNameAsync(query);

            Console.WriteLine($"[SearchProduto] Produtos encontrados: {produtos.Count}"); // <- log quantidade de produtos

            return Ok(produtos); // garante 200 OK com JSON
        }



        // CRIAR (GET)
        public IActionResult Create()
        {
            ViewBag.Pessoas = _context.Pessoas.ToList();
            ViewBag.TiposMovimento = _context.TiposMovimento.ToList();
            ViewBag.Produtos = _context.Produtos.ToList();
            ViewBag.ProdutoEmbalagens = _context.ProdutosEmbalagem
             .Include(pe => pe.TipoEmbalagem)
             .ToList();
            return View(new MovimentoViewModel());
        }

        // CRIAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovimentoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Pessoas = _context.Pessoas.ToList();
                ViewBag.TiposMovimento = _context.TiposMovimento.ToList();
                ViewBag.Produtos = _context.Produtos.ToList();
                return View(vm);
            }

            // Criar Movimento
            var movimento = new Movimento
            {
                Documento = vm.Documento,
                Observacao = vm.Observacao,
                DataMovimento = DateTime.Now,
                TipoMovimentoId = vm.TipoMovimentoId,
                PessoaId = vm.PessoaId
            };

            _context.Movimentos.Add(movimento);
            await _context.SaveChangesAsync(); // salva para gerar IdMovimento

            // Criar ProdutoMovimento e atualizar estoque
            foreach (var item in vm.Produtos)
            {
                var produtoMovimento = new ProdutoMovimento
                {
                    MovimentoId = movimento.IdMovimento,
                    ProdutoId = item.ProdutoId,
                    ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                    Quantidade = item.Quantidade
                };

                _context.ProdutosMovimento.Add(produtoMovimento);
                await _produtoMovimentoRepository.AtualizarEstoqueAsync(produtoMovimento);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

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
        private readonly IEstoqueRepository _estoqueRepo; // Adicionado para buscar dados do produto

        public MovimentoController(VarejoDbContext context,
            IProdutoMovimentoRepository produtoMovimentoRepository,
            IProdutoRepository produtoRepository,
            IEstoqueRepository estoqueRepo)
        {
            _context = context;
            _produtoMovimentoRepository = produtoMovimentoRepository;
            _produtoRepository = produtoRepository;
            _estoqueRepo = estoqueRepo;
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
                ViewBag.ProdutoEmbalagens = _context.ProdutosEmbalagem.Include(pe => pe.TipoEmbalagem).ToList();
                return View(vm);
            }

            // 1. Criar e Salvar o Cabeçalho do Movimento
            var movimento = new Movimento
            {
                Documento = vm.Documento,
                Observacao = vm.Observacao ?? "Movimentação Manual",
                DataMovimento = DateTime.Now,
                TipoMovimentoId = vm.TipoMovimentoId,
                PessoaId = vm.PessoaId
            };

            _context.Movimentos.Add(movimento);
            await _context.SaveChangesAsync(); // Gera o IdMovimento necessário para o Histórico

            // 2. Processar os Itens
            foreach (var item in vm.Produtos)
            {
                // Criar o registro na tabela de detalhe (ProdutoMovimento)
                var produtoMovimento = new ProdutoMovimento
                {
                    MovimentoId = movimento.IdMovimento,
                    ProdutoId = item.ProdutoId,
                    ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                    Quantidade = item.Quantidade
                };
                _context.ProdutosMovimento.Add(produtoMovimento);

                // 3. O PONTO CHAVE: Registrar no Histórico e Atualizar Saldo do Produto
                // Passamos a quantidade exatamente como ela é. 
                // Se o seu Repository já trata o sinal (+ ou -) baseado no TipoMovimentoId, ótimo.
                // Se não, podemos passar (movimento.TipoMovimentoId == 2 ? -item.Quantidade : item.Quantidade)

                await _estoqueRepo.RegistrarMovimentacaoAsync(
                    item.ProdutoId,
                    movimento.IdMovimento,
                    movimento.TipoMovimentoId,
                    item.Quantidade,
                    movimento.Observacao
                );
            }

            // 4. Salva todas as alterações de uma vez
            await _context.SaveChangesAsync();

            TempData["Success"] = "Movimentação realizada e histórico atualizado!";
            return RedirectToAction(nameof(Index));
        }
    }

}

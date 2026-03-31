using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class VendasController : Controller
    {
        private readonly IVendaRepository _vendaRepo;
        private readonly IPessoaRepository _pessoaRepo; // Você deve ter esse para Clientes
        private readonly IFormaPagamentoRepository _formaRepo;
        private readonly IPrazoPagamentoRepository _prazoRepo;
        private readonly VarejoDbContext _context; // Para buscas rápidas via LINQ

        public VendasController(
            IVendaRepository vendaRepo,
            IPessoaRepository pessoaRepo,
            IFormaPagamentoRepository formaRepo,
            IPrazoPagamentoRepository prazoRepo,
            VarejoDbContext context)
        {
            _vendaRepo = vendaRepo;
            _pessoaRepo = pessoaRepo;
            _formaRepo = formaRepo;
            _prazoRepo = prazoRepo;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vendas = await _vendaRepo.GetAllAsync();
            return View(vendas);
        }

        // GET: Vendas/Create
        public async Task<IActionResult> Create()
        {
            await CarregarViewBags();
            return View(new VendaViewModel());
        }

        // POST: Vendas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendaViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Mapeamento manual de ViewModel para Model
                var venda = new Venda
                {
                    PessoaId = vm.PessoaId,
                    FormaPagamentoId = vm.FormaPagamentoId,
                    PrazoPagamentoId = vm.PrazoPagamentoId,
                    DataVenda = DateTime.Now,
                    Observacao = vm.Observacao,
                    ValorSubtotal = vm.Itens.Sum(i => i.Subtotal),
                    DescontoTotal = vm.DescontoTotal,
                    Finalizada = false // Salva apenas como pedido inicialmente
                };

                foreach (var itemVm in vm.Itens)
                {
                    venda.Itens.Add(new VendaItem
                    {
                        ProdutoId = itemVm.ProdutoId,
                        ProdutoEmbalagemId = itemVm.ProdutoEmbalagemId,
                        Quantidade = itemVm.Quantidade,
                        ValorUnitario = itemVm.ValorUnitario,
                        DescontoUnitario = itemVm.DescontoUnitario
                    });
                }

                await _vendaRepo.CriarPedidoAsync(venda);
                return RedirectToAction(nameof(Index));
            }

            await CarregarViewBags();
            return View(vm);
        }

        // Método auxiliar para carregar os Dropdowns
        private async Task CarregarViewBags()
        {
            ViewBag.Clientes = new SelectList(await _pessoaRepo.GetClientesAtivosAsync(), "IdPessoa", "NomeRazao");
            ViewBag.FormasPagamento = new SelectList(await _formaRepo.GetAllAsync(), "IdFormaPagamento", "DescricaoFormaPagamento");
            ViewBag.PrazosPagamento = new SelectList(await _prazoRepo.GetAllAsync(), "IdPrazoPagamento", "Descricao");
        }


        // Endpoint para o AJAX da View buscar as embalagens de um produto
        [HttpGet]
        public async Task<IActionResult> GetEmbalagensPorProduto(int produtoId)
        {
            var embalagens = await _context.ProdutosEmbalagem
                .Include(e => e.TipoEmbalagem)
                .Where(e => e.ProdutoId == produtoId)
                .Select(e => new {
                    id = e.IdProdutoEmbalagem,
                    descricao = e.TipoEmbalagem.DescricaoTipoEmbalagem,
                    preco = e.Preco,
                    ean = e.Ean
                })
                .ToListAsync();

            return Json(embalagens);
        }


    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class VendaController : Controller
    {
        private readonly IVendaRepository _vendaRepo;
        private readonly IPessoaRepository _pessoaRepo;
        private readonly IFormaPagamentoRepository _formaRepo;
        private readonly IPrazoPagamentoRepository _prazoRepo;
        private readonly IProdutoRepository _produtoRepo; // Adicionado para usar seu GetByNameAsync
        private readonly VarejoDbContext _context;

        public VendaController(
            IVendaRepository vendaRepo,
            IPessoaRepository pessoaRepo,
            IFormaPagamentoRepository formaRepo,
            IPrazoPagamentoRepository prazoRepo,
            IProdutoRepository produtoRepo,
            VarejoDbContext context
            )
        {
            _vendaRepo = vendaRepo;
            _pessoaRepo = pessoaRepo;
            _formaRepo = formaRepo;
            _prazoRepo = prazoRepo;
            _produtoRepo = produtoRepo;
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? dataInicio, DateTime? dataFim, string cliente, string status)
        {
            var query = _context.Vendas
                .Include(v => v.Pessoa)
                .Include(v => v.FormaPagamento)
                .AsQueryable();

            // Aplicação dos Filtros
            if (dataInicio.HasValue)
                query = query.Where(v => v.DataVenda.Date >= dataInicio.Value.Date);

            if (dataFim.HasValue)
                query = query.Where(v => v.DataVenda.Date <= dataFim.Value.Date);

            if (!string.IsNullOrEmpty(cliente))
                query = query.Where(v => v.Pessoa.NomeRazao.Contains(cliente));

            if (status == "finalizada")
                query = query.Where(v => v.Finalizada == true);
            else if (status == "aberta")
                query = query.Where(v => v.Finalizada == false);

            var resultado = await query.OrderByDescending(v => v.DataVenda).ToListAsync();
            return View(resultado);
        }

        public async Task<IActionResult> Create()
        {
            await CarregarViewBags();
            return View(new VendaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Isso aqui vai te mostrar no console do Visual Studio EXATAMENTE qual campo está falhando
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine("ERRO MODELSTATE: " + error.ErrorMessage);
                }

       
            }


            if (ModelState.IsValid)
            {
                try
                {
                    var venda = new Venda
                    {
                        PessoaId = vm.PessoaId,
                        FormaPagamentoId = vm.FormaPagamentoId,
                        PrazoPagamentoId = vm.PrazoPagamentoId,
                        DataVenda = DateTime.Now,
                        Observacao = vm.Observacao,
                        ValorSubtotal = vm.Itens.Sum(i => i.Subtotal),
                        DescontoTotal = vm.DescontoTotal,
                        Finalizada = false
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
                    return RedirectToAction(nameof(Details), new { id = venda.IdVenda });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar: " + ex.Message);
                }
            }

            await CarregarViewBags();
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var venda = await _vendaRepo.GetByIdAsync(id);
            if (venda == null) return NotFound();
            return View(venda);
        }

        [HttpPost]
        public async Task<IActionResult> Faturar(int id)
        {
            try
            {
                // Este método no VendaRepository agora orquestra o EstoqueRepository e TituloFinanceiroRepository
                var sucesso = await _vendaRepo.FaturarVendaAsync(id);
                if (sucesso) TempData["Sucesso"] = "Venda faturada, estoque baixado e financeiro gerado!";
                else TempData["Erro"] = "Venda não encontrada ou já finalizada.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro no faturamento: " + ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // --- ENDPOINTS PARA AJAX (TELA DE VENDA) ---

        [HttpGet]
        public async Task<IActionResult> BuscarProdutos(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return Ok(new List<ProdutoViewModel>());

            // O seu repositório já faz o Select para ProdutoViewModel e traz as Embalagens
            var produtos = await _produtoRepo.GetByNameAsync(termo);

            return Ok(produtos);
        }

        private async Task CarregarViewBags()
        {
            // Busca Clientes usando o método que adicionamos ao PessoaRepository
            ViewBag.Clientes = new SelectList(await _pessoaRepo.GetClientesAtivosAsync(), "IdPessoa", "NomeRazao");

            ViewBag.FormasPagamento = new SelectList(await _formaRepo.GetAllAsync(), "IdFormaPagamento", "DescricaoFormaPagamento");
            ViewBag.PrazosPagamento = new SelectList(await _prazoRepo.GetAllAsync(), "IdPrazoPagamento", "Descricao");
        }
    }
}
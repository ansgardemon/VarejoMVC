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
        private readonly IEstoqueRepository _estoqueRepo; // Adicionado para buscar dados do produto
        private readonly VarejoDbContext _context;

        public VendaController(
            IVendaRepository vendaRepo,
            IPessoaRepository pessoaRepo,
            IFormaPagamentoRepository formaRepo,
            IPrazoPagamentoRepository prazoRepo,
            IProdutoRepository produtoRepo,
            IEstoqueRepository estoqueRepo,
            VarejoDbContext context
            )
        {
            _vendaRepo = vendaRepo;
            _pessoaRepo = pessoaRepo;
            _formaRepo = formaRepo;
            _prazoRepo = prazoRepo;
            _produtoRepo = produtoRepo;
            _context = context;
            _estoqueRepo = estoqueRepo;
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
            // O segredo está nestes .ThenInclude para carregar os nomes
            var venda = await _context.Vendas
                .Include(v => v.Pessoa)
                .Include(v => v.FormaPagamento)
                .Include(v => v.PrazoPagamento)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto) // Carrega o objeto Produto para pegar o Nome
                .Include(v => v.Itens)
                    .ThenInclude(i => i.ProdutoEmbalagem)
                        .ThenInclude(e => e.TipoEmbalagem) // Carrega a descrição da embalagem
                .FirstOrDefaultAsync(m => m.IdVenda == id);

            if (venda == null) return NotFound();

            var model = new VendaViewModelDetails
            {
                IdVenda = venda.IdVenda,
                PessoaId = venda.PessoaId,
                FormaPagamentoId = venda.FormaPagamentoId,
                PrazoPagamentoId = venda.PrazoPagamentoId,
                Observacao = venda.Observacao,
                ValorSubtotal = venda.ValorSubtotal,
                DescontoTotal = venda.DescontoTotal,
                Finalizada = venda.Finalizada,

                // Aqui resolvemos o "vermelho":
                Itens = venda.Itens.Select(i => new VendaItemViewModelDetails
                {
                    ProdutoId = i.ProdutoId,
                    // Buscamos o nome direto da navegação do objeto de banco
                    NomeProduto = i.Produto?.NomeProduto ?? "Produto não encontrado",

                    ProdutoEmbalagemId = i.ProdutoEmbalagemId,
                    // Buscamos a descrição da embalagem da mesma forma
                    NomeEmbalagem = i.ProdutoEmbalagem?.TipoEmbalagem?.DescricaoTipoEmbalagem ?? "Unidade",

                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    DescontoUnitario = i.DescontoUnitario
                }).ToList()
            };

            ViewBag.NomeCliente = venda.Pessoa?.NomeRazao ?? "Consumidor";
            ViewBag.DescricaoPagamento = venda.FormaPagamento?.DescricaoFormaPagamento;
            ViewBag.DescricaoPrazo = venda.PrazoPagamento?.Descricao ?? "À Vista";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Faturar(int id)
        {
            try
            {
                // Chama o repositório que agora orquestra TUDO corretamente
                var sucesso = await _vendaRepo.FaturarVendaAsync(id);

                if (sucesso)
                {
                    TempData["Sucesso"] = "Venda faturada com sucesso! Estoque e Financeiro atualizados.";
                    return RedirectToAction(nameof(Index)); // Volta pro Index após sucesso
                }

                TempData["Erro"] = "Venda não encontrada ou já finalizada.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro no faturamento: " + ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Imprimir(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Pessoa)
                .Include(v => v.FormaPagamento)
                .Include(v => v.PrazoPagamento)
                .Include(v => v.Itens).ThenInclude(i => i.Produto)
                .Include(v => v.Itens).ThenInclude(i => i.ProdutoEmbalagem).ThenInclude(e => e.TipoEmbalagem)
                .FirstOrDefaultAsync(m => m.IdVenda == id);

            if (venda == null) return NotFound();

            var model = new VendaViewModelDetails
            {
                IdVenda = venda.IdVenda,
                PessoaId = venda.PessoaId,
                FormaPagamentoId = venda.FormaPagamentoId,
                PrazoPagamentoId = venda.PrazoPagamentoId,
                Observacao = venda.Observacao,
                ValorSubtotal = venda.ValorSubtotal,
                DescontoTotal = venda.DescontoTotal,
                Finalizada = venda.Finalizada,
                Itens = venda.Itens.Select(i => new VendaItemViewModelDetails
                {
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.Produto?.NomeProduto ?? "Produto não encontrado",
                    NomeEmbalagem = i.ProdutoEmbalagem?.TipoEmbalagem?.DescricaoTipoEmbalagem ?? "UN",
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    DescontoUnitario = i.DescontoUnitario
                }).ToList()
            };

            ViewBag.NomeCliente = venda.Pessoa?.NomeRazao ?? "CONSUMIDOR FINAL";
            ViewBag.DocCliente = venda.Pessoa?.CpfCnpj ?? "000.000.000-00";
            ViewBag.DataVenda = venda.DataVenda.ToString("dd/MM/yyyy HH:mm");
            ViewBag.FormaPagto = venda.FormaPagamento?.DescricaoFormaPagamento ?? "N/A";
            ViewBag.Prazo = venda.PrazoPagamento?.Descricao ?? "À VISTA";

            return View(model);
        }



        // GET: Venda/Cancelar/5
        [HttpGet]
        public async Task<IActionResult> Cancelar(int id)
        {
            // Buscamos a venda para mostrar os dados na tela de confirmação
            var venda = await _context.Vendas
                .Include(v => v.Pessoa)
                .FirstOrDefaultAsync(m => m.IdVenda == id);

            if (venda == null) return NotFound();

            // Regra de segurança: Se já estiver faturada, não pode cancelar por aqui
            if (venda.Finalizada)
            {
                TempData["Erro"] = "Esta venda já foi finalizada e não pode ser removida.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(venda);
        }

        // POST: Venda/Cancelar/5
        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarConfirmado(int id)
        {
            try
            {
                // Usando o seu método do repositório
                var sucesso = await _vendaRepo.CancelarVendaAsync(id);

                if (sucesso)
                {
                    TempData["Sucesso"] = "Venda cancelada e removida com sucesso.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Erro"] = "Não foi possível cancelar a venda. Verifique se ela já foi finalizada.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro técnico ao cancelar: " + ex.Message;
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

        [HttpGet]
        public async Task<IActionResult> BuscarClientes(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return Ok(new List<object>());

            // Usa o método que você acabou de adicionar ao repositório
            var clientes = await _pessoaRepo.SearchClientesAsync(termo);

            // Retornamos um objeto anônimo simples para o JS processar
            var resultado = clientes.Select(c => new
            {
                idPessoa = c.IdPessoa,
                nomeRazao = c.NomeRazao,
                cpfCnpj = c.CpfCnpj
            });

            return Ok(resultado);
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    [Authorize(Roles = "Administrador, Gerente")]
    public class TituloFinanceiroController : Controller
    {
        private readonly ITituloFinanceiroRepository _repo;
        private readonly IEspecieTituloRepository _especieRepo;
        private readonly IFormaPagamentoRepository _formaRepo;
        private readonly IPrazoPagamentoRepository _prazoRepo;

        public TituloFinanceiroController(
            ITituloFinanceiroRepository repo,
            IEspecieTituloRepository especieRepo,
            IFormaPagamentoRepository formaRepo,
            IPrazoPagamentoRepository prazoRepo)
        {
            _repo = repo;
            _especieRepo = especieRepo;
            _formaRepo = formaRepo;
            _prazoRepo = prazoRepo;
        }

        // =========================
        // HELPERS (ViewBags)
        // =========================
        private async Task CarregarCombos()
        {
            Console.WriteLine("[DEBUG] Carregando combos...");

            var especies = await _especieRepo.GetAllAsync();
            var formas = await _formaRepo.GetAllAsync();
            var prazos = await _prazoRepo.GetAllAsync();

            Console.WriteLine($"[DEBUG] Espécies: {especies?.Count ?? 0}");
            Console.WriteLine($"[DEBUG] Formas: {formas?.Count ?? 0}");
            Console.WriteLine($"[DEBUG] Prazos: {prazos?.Count ?? 0}");

            ViewBag.Especies = new SelectList(especies ?? new List<EspecieTitulo>(), "IdEspecieTitulo", "Descricao");
            ViewBag.Formas = new SelectList(formas ?? new List<FormaPagamento>(), "IdFormaPagamento", "DescricaoFormaPagamento");
            ViewBag.Prazos = new SelectList(prazos ?? new List<PrazoPagamento>(), "IdPrazoPagamento", "Descricao");
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index(string documento, string pessoa, string status)
        {
            var titulos = await _repo.GetAllAsync();

            var dataLimite = DateTime.Now.AddDays(30);

            var query = titulos.AsQueryable();

            if (string.IsNullOrEmpty(status) || status == "aberto")
            {
                query = query.Where(t => !t.Quitado && t.DataVencimento <= dataLimite);
            }
            else if (status == "quitado")
            {
                query = query.Where(t => t.Quitado);
            }

            if (!string.IsNullOrEmpty(documento))
            {
                query = query.Where(t => t.Documento.ToString().Contains(documento));
            }

            if (!string.IsNullOrEmpty(pessoa))
            {
                query = query.Where(t => t.Pessoa != null && t.Pessoa.NomeRazao.Contains(pessoa));
            }

            var vm = query.Select(t => new TituloFinanceiroViewModel
            {
                IdTituloFinanceiro = t.IdTituloFinanceiro,
                Documento = t.Documento,
                Parcela = t.Parcela,
                Valor = t.Valor,
                ValorPago = t.ValorPago,
                ValorAberto = t.ValorAberto,
                DataVencimento = t.DataVencimento,
                Quitado = t.Quitado,
                NomePessoa = t.Pessoa != null ? t.Pessoa.NomeRazao : "",
                EspecieDescricao = t.EspecieTitulo != null ? t.EspecieTitulo.Descricao : ""
            }).ToList();

            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            Console.WriteLine("[DEBUG] GET Create chamado");
            await CarregarCombos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TituloFinanceiroCreateViewModel viewModel)
        {
            Console.WriteLine("[DEBUG] POST Create chamado");

            // ===== DADOS RECEBIDOS =====
            Console.WriteLine($"Documento: {viewModel.Documento}");
            Console.WriteLine($"Parcela: {viewModel.Parcela}");
            Console.WriteLine($"PessoaId: {viewModel.PessoaId}");
            Console.WriteLine($"EspecieTituloId: {viewModel.EspecieTituloId}");
            Console.WriteLine($"FormaPagamentoId: {viewModel.FormaPagamentoId}");
            Console.WriteLine($"PrazoPagamentoId: {viewModel.PrazoPagamentoId}");
            Console.WriteLine($"Valor: {viewModel.Valor}");
            Console.WriteLine($"DataEmissao: {viewModel.DataEmissao}");
            Console.WriteLine($"DataVencimento: {viewModel.DataVencimento}");

            // ===== VALIDAR MODESTATE =====
            if (!ModelState.IsValid)
            {
                Console.WriteLine("[DEBUG] ModelState inválido");

                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        Console.WriteLine($"Erro em {item.Key}: {error.ErrorMessage}");
                    }
                }

                await CarregarCombos();
                return View(viewModel);
            }

            Console.WriteLine("[DEBUG] ModelState válido");

            if (viewModel.PessoaId == null)
            {
                Console.WriteLine("[DEBUG] PessoaId está NULL");
            }

            var titulo = new TituloFinanceiro
            {
                Documento = viewModel.Documento,
                Parcela = viewModel.Parcela,
                Observacao = viewModel.Observacao,
                Valor = viewModel.Valor,
                ValorPago = viewModel.ValorPago,
                DataEmissao = viewModel.DataEmissao,
                DataVencimento = viewModel.DataVencimento,
                DataPagamento = viewModel.DataPagamento,
                EspecieTituloId = viewModel.EspecieTituloId,
                FormaPagamentoId = viewModel.FormaPagamentoId,
                PrazoPagamentoId = viewModel.PrazoPagamentoId,
                PessoaId = viewModel.PessoaId
            };

            titulo.AtualizarValores();

            Console.WriteLine("[DEBUG] Entidade montada:");
            Console.WriteLine($"ValorAberto: {titulo.ValorAberto}");
            Console.WriteLine($"Quitado: {titulo.Quitado}");

            try
            {
                Console.WriteLine("[DEBUG] Salvando no banco...");
                await _repo.AddAsync(titulo);
                Console.WriteLine("[DEBUG] Salvou com sucesso!");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERRO] " + ex.Message);
                Console.WriteLine("[ERRO STACK] " + ex.StackTrace);

                ModelState.AddModelError("", "Erro ao salvar título.");

                await CarregarCombos();
                return View(viewModel);
            }
        }
    

        // =========================
        // EDIT (GET)
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var t = await _repo.GetByIdAsync(id);
            if (t == null) return NotFound();

            await CarregarCombos();

            var vm = new TituloFinanceiroViewModel
            {
                IdTituloFinanceiro = t.IdTituloFinanceiro,
                Documento = t.Documento,
                Parcela = t.Parcela,
                Observacao = t.Observacao,
                Valor = t.Valor,
                ValorPago = t.ValorPago,
                DataEmissao = t.DataEmissao,
                DataVencimento = t.DataVencimento,
                DataPagamento = t.DataPagamento,
                EspecieTituloId = t.EspecieTituloId,
                FormaPagamentoId = t.FormaPagamentoId,
                PrazoPagamentoId = t.PrazoPagamentoId,
                PessoaId = t.PessoaId
            };

            return View(vm);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TituloFinanceiroViewModel viewModel)
        {
            if (id != viewModel.IdTituloFinanceiro)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await CarregarCombos();
                return View(viewModel);
            }

            var titulo = new TituloFinanceiro
            {
                IdTituloFinanceiro = viewModel.IdTituloFinanceiro,
                Documento = viewModel.Documento,
                Parcela = viewModel.Parcela,
                Observacao = viewModel.Observacao,
                Valor = viewModel.Valor,
                ValorPago = viewModel.ValorPago,
                DataEmissao = viewModel.DataEmissao,
                DataVencimento = viewModel.DataVencimento,
                DataPagamento = viewModel.DataPagamento,
                EspecieTituloId = viewModel.EspecieTituloId,
                FormaPagamentoId = viewModel.FormaPagamentoId,
                PrazoPagamentoId = viewModel.PrazoPagamentoId,
                PessoaId = viewModel.PessoaId
            };

            titulo.AtualizarValores();

            try
            {
                await _repo.UpdateAsync(titulo);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERRO] " + ex.Message);
                ModelState.AddModelError("", "Erro ao atualizar título.");
                await CarregarCombos();
                return View(viewModel);
            }
        }
    }
}
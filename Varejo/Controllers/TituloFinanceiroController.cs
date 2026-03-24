using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class TituloFinanceiroController : Controller
    {
        private readonly ITituloFinanceiroRepository _repo;

        public TituloFinanceiroController(ITituloFinanceiroRepository repo)
        {
            _repo = repo;
        }

        // =========================
        // INDEX
        // =========================
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Index(string documento, string pessoa, string status)
        {
            var titulos = await _repo.GetAllAsync();

            // 🔹 filtro padrão: próximos 30 dias + em aberto
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
                EspecieDescricao = t.EspecieTitulo.Descricao
            }).ToList();

            return View(vm);
        }

        // =========================
        // CREATE (GET)
        // =========================
        [Authorize(Roles = "Administrador, Gerente")]
        public IActionResult Create()
        {
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TituloFinanceiroViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
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

                try
                {
                    await _repo.AddAsync(titulo);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] Ao criar título: " + ex.Message);
                    ModelState.AddModelError("", "Erro ao salvar título financeiro.");
                }
            }

            return View(viewModel);
        }

        // =========================
        // EDIT (GET)
        // =========================
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            var t = await _repo.GetByIdAsync(id);
            if (t == null)
                return NotFound();

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

            if (ModelState.IsValid)
            {
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
                    Console.WriteLine("[ERRO] Ao atualizar título: " + ex.Message);
                    ModelState.AddModelError("", "Erro ao atualizar título.");
                }
            }

            return View(viewModel);
        }
    }
}
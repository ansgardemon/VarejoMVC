using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
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
        private readonly IPagamentoTituloRepository _pagamentoRepo;

        public TituloFinanceiroController(
            ITituloFinanceiroRepository repo,
            IEspecieTituloRepository especieRepo,
            IFormaPagamentoRepository formaRepo,
            IPrazoPagamentoRepository prazoRepo,
            IPagamentoTituloRepository pagamentoRepo)
        {
            _repo = repo;
            _especieRepo = especieRepo;
            _formaRepo = formaRepo;
            _prazoRepo = prazoRepo;
            _pagamentoRepo = pagamentoRepo;
        }


        // =========================
        // HELPERS
        // =========================
        private async Task CarregarCombos()
        {
            var especies = await _especieRepo.GetAllAsync();
            var formas = await _formaRepo.GetAllAsync();
            var prazos = await _prazoRepo.GetAllAsync();

            ViewBag.Especies = new SelectList(especies, "IdEspecieTitulo", "Descricao");
            ViewBag.Formas = new SelectList(formas, "IdFormaPagamento", "DescricaoFormaPagamento");
            ViewBag.Prazos = new SelectList(prazos, "IdPrazoPagamento", "Descricao");
        }

        // =========================
        // INDEX (com filtro padrão de 30 dias)
        // =========================
        public async Task<IActionResult> Index(
            string documento,
            string pessoa,
            string status,
            DateTime? dataInicio,
            DateTime? dataFim)
        {
            var titulos = await _repo.GetAllAsync();

            var query = titulos.AsQueryable();

            // PERÍODO PADRÃO (30 dias)
            if (!dataInicio.HasValue && !dataFim.HasValue)
            {
                dataInicio = DateTime.Today;
                dataFim = DateTime.Today.AddDays(30);
            }

            if (dataInicio.HasValue)
                query = query.Where(t => t.DataVencimento >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(t => t.DataVencimento <= dataFim.Value);

            // STATUS
            if (string.IsNullOrEmpty(status) || status == "aberto")
                query = query.Where(t => !t.Quitado);
            else if (status == "quitado")
                query = query.Where(t => t.Quitado);

            // DOCUMENTO
            if (!string.IsNullOrEmpty(documento))
                query = query.Where(t => t.Documento.ToString().Contains(documento));

            // PESSOA
            if (!string.IsNullOrEmpty(pessoa))
                query = query.Where(t => t.Pessoa != null && t.Pessoa.NomeRazao.Contains(pessoa));

            var vm = query
                .Select(t => new TituloFinanceiroViewModel
            {
                IdTituloFinanceiro = t.IdTituloFinanceiro,
                Documento = t.Documento,
                Parcela = t.Parcela,
                Valor = t.Valor,
                ValorPago = t.Pagamentos.Sum(p => p.ValorPago),
                ValorAberto = t.Valor - t.Pagamentos.Sum(p => p.ValorPago),
                DataVencimento = t.DataVencimento,
                Quitado = t.Quitado,
                NomePessoa = t.Pessoa != null ? t.Pessoa.NomeRazao : "",
                EspecieDescricao = t.EspecieTitulo != null ? t.EspecieTitulo.Descricao : ""
            }).ToList();


            return View(vm);
        }

        // =========================
        // CREATE
        // =========================
        public async Task<IActionResult> Create()
        {
            await CarregarCombos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TituloFinanceiroCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await CarregarCombos();
                return View(viewModel);
            }

            var titulo = new TituloFinanceiro
            {
                Documento = viewModel.Documento,
                Parcela = viewModel.Parcela,
                Observacao = viewModel.Observacao,
                Valor = viewModel.Valor,
                DataEmissao = viewModel.DataEmissao,
                DataVencimento = viewModel.DataVencimento,
                EspecieTituloId = viewModel.EspecieTituloId,
                FormaPagamentoId = viewModel.FormaPagamentoId,
                PrazoPagamentoId = viewModel.PrazoPagamentoId,
                PessoaId = viewModel.PessoaId
            };

            titulo.AtualizarValores();

            await _repo.AddAsync(titulo);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS (com histórico)
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var t = await _repo.GetByIdAsync(id);
            if (t == null) return NotFound();

            var vm = new TituloFinanceiroViewModel
            {
                IdTituloFinanceiro = t.IdTituloFinanceiro,
                Documento = t.Documento,
                Parcela = t.Parcela,
                Observacao = t.Observacao,
                Valor = t.Valor,
                ValorPago = t.Pagamentos != null
    ? t.Pagamentos.Sum(p => p.ValorPago)
    : 0,
                ValorAberto = t.ValorAberto,
                DataEmissao = t.DataEmissao,
                DataVencimento = t.DataVencimento,
                Quitado = t.Quitado,
                NomePessoa = t.Pessoa?.NomeRazao,
                EspecieDescricao = t.EspecieTitulo?.Descricao,
                FormaDescricao = t.FormaPagamento != null ? t.FormaPagamento.DescricaoFormaPagamento : "",
            };

            ViewBag.Pagamentos = t.Pagamentos
                .OrderByDescending(p => p.DataPagamento)
                .ToList();

            return View(vm);
        }

        // =========================
        // REGISTRAR PAGAMENTO
        // =========================
        [HttpPost]
        public async Task<IActionResult> RegistrarPagamento(int id, string valor, DateTime data)
        {
            try
            {
                var valorDecimal = decimal.Parse(valor, new CultureInfo("pt-BR"));

                await _pagamentoRepo.RegistrarPagamentoAsync(id, valorDecimal, data);

                TempData["Sucesso"] = "Pagamento registrado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // =========================
        // EDIT
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
                DataEmissao = t.DataEmissao,
                DataVencimento = t.DataVencimento,
                EspecieTituloId = t.EspecieTituloId,
                FormaPagamentoId = t.FormaPagamentoId,
                PrazoPagamentoId = t.PrazoPagamentoId,
                PessoaId = t.PessoaId
            };

            return View(vm);
        }

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

            var titulo = await _repo.GetByIdAsync(id);

            if (titulo == null)
                return NotFound();

            //  ATUALIZA OS CAMPOS EDITÁVEIS
            titulo.Documento = viewModel.Documento;
            titulo.Parcela = viewModel.Parcela;
            titulo.Observacao = viewModel.Observacao;
            titulo.Valor = viewModel.Valor;
            titulo.DataEmissao = viewModel.DataEmissao;
            titulo.DataVencimento = viewModel.DataVencimento;

            //  NÃO mexe nos FKs (porque não estão na view)

            titulo.AtualizarValores();

            await _repo.UpdateAsync(titulo);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _repo.GetByIdAsync(id);
            if (t == null) return NotFound();

            if (t.Pagamentos.Any())
            {
                TempData["Erro"] = "Não é possível excluir um título com pagamentos registrados.";
                return RedirectToAction(nameof(Index));
            }

            return View(t);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var t = await _repo.GetByIdAsync(id);

            if (t == null)
                return NotFound();

            if (t.Pagamentos.Any())
            {
                TempData["Erro"] = "Não é possível excluir um título com pagamentos registrados.";
                return RedirectToAction(nameof(Index));
            }

            await _repo.DeleteAsync(id);

            TempData["Sucesso"] = "Título excluído com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // CALENDÁRIO TOP DEMAIS BOY SÉLOCO

        public IActionResult Calendar()
        {
            return View();
        }

        public async Task<IActionResult> GetEventos(DateTime? start, DateTime? end)
        {
            var titulos = await _repo.GetAllAsync();

            var eventos = titulos.Select(t => new
            {
                id = t.IdTituloFinanceiro,
                title = $"{t.Documento} - {t.Parcela} | {t.ValorAberto:C}",
                start = t.DataVencimento.ToString("yyyy-MM-dd"),
                color = t.Quitado ? "#28a745" : "#dc3545"
            });

            return Json(eventos);
        }
    }
}
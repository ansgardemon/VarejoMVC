using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class PrazoPagamentoController : Controller
    {
        private readonly IPrazoPagamentoRepository _repo;

        public PrazoPagamentoController(IPrazoPagamentoRepository repo)
        {
            _repo = repo;
        }

        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Index()
        {
            var lista = await _repo.GetAllAsync();

            var vm = lista.Select(p => new PrazoPagamentoViewModel
            {
                IdPrazoPagamento = p.IdPrazoPagamento,
                Descricao = p.Descricao,
                NumeroParcelas = p.NumeroParcelas,
                IntervaloDias = p.IntervaloDias
            }).ToList();

            return View(vm);
        }

        [Authorize(Roles = "Administrador, Gerente")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrazoPagamentoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var model = new PrazoPagamento
                {
                    Descricao = viewModel.Descricao,
                    NumeroParcelas = viewModel.NumeroParcelas,
                    IntervaloDias = viewModel.IntervaloDias
                };

                try
                {
                    await _repo.AddAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] PrazoPagamento Create: " + ex.Message);
                    ModelState.AddModelError("", "Erro ao criar prazo.");
                }
            }

            return View(viewModel);
        }

        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            var vm = new PrazoPagamentoViewModel
            {
                IdPrazoPagamento = item.IdPrazoPagamento,
                Descricao = item.Descricao,
                NumeroParcelas = item.NumeroParcelas,
                IntervaloDias = item.IntervaloDias
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PrazoPagamentoViewModel viewModel)
        {
            if (id != viewModel.IdPrazoPagamento)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var model = new PrazoPagamento
                {
                    IdPrazoPagamento = viewModel.IdPrazoPagamento,
                    Descricao = viewModel.Descricao,
                    NumeroParcelas = viewModel.NumeroParcelas,
                    IntervaloDias = viewModel.IntervaloDias
                };

                try
                {
                    await _repo.UpdateAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] PrazoPagamento Edit: " + ex.Message);
                    ModelState.AddModelError("", "Erro ao atualizar.");
                }
            }

            return View(viewModel);
        }

        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            var vm = new PrazoPagamentoViewModel
            {
                IdPrazoPagamento = item.IdPrazoPagamento,
                Descricao = item.Descricao,
                NumeroParcelas = item.NumeroParcelas,
                IntervaloDias = item.IntervaloDias
            };

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _repo.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERRO] PrazoPagamento Delete: " + ex.Message);
                ViewData["DeleteError"] = "Não foi possível excluir.";

                var item = await _repo.GetByIdAsync(id);

                var vm = new PrazoPagamentoViewModel
                {
                    IdPrazoPagamento = item.IdPrazoPagamento,
                    Descricao = item.Descricao,
                    NumeroParcelas = item.NumeroParcelas,
                    IntervaloDias = item.IntervaloDias
                };

                return View("Delete", vm);
            }
        }
    }
}
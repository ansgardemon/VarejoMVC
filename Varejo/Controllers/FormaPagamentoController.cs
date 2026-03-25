using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class FormaPagamentoController : Controller
    {
        private readonly IFormaPagamentoRepository _repo;

        public FormaPagamentoController(IFormaPagamentoRepository repo)
        {
            _repo = repo;
        }

        // =========================
        // INDEX
        // =========================
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Index()
        {
            var lista = await _repo.GetAllAsync();

            var vm = lista.Select(f => new FormaPagamentoViewModel
            {
                IdFormaPagamento = f.IdFormaPagamento,
                DescricaoFormaPagamento = f.DescricaoFormaPagamento
            }).ToList();

            return View(vm);
        }

        // =========================
        // CREATE
        // =========================
        [Authorize(Roles = "Administrador, Gerente")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormaPagamentoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var model = new FormaPagamento
                {
                    DescricaoFormaPagamento = viewModel.DescricaoFormaPagamento
                };

                try
                {
                    await _repo.AddAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] FormaPagamento Create: " + ex.Message);
                    ModelState.AddModelError("", "Erro ao criar forma de pagamento.");
                }
            }

            return View(viewModel);
        }

        // =========================
        // EDIT
        // =========================
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            var vm = new FormaPagamentoViewModel
            {
                IdFormaPagamento = item.IdFormaPagamento,
                DescricaoFormaPagamento = item.DescricaoFormaPagamento
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FormaPagamentoViewModel viewModel)
        {
            if (id != viewModel.IdFormaPagamento)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var model = new FormaPagamento
                {
                    IdFormaPagamento = viewModel.IdFormaPagamento,
                    DescricaoFormaPagamento = viewModel.DescricaoFormaPagamento
                };

                try
                {
                    await _repo.UpdateAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] FormaPagamento Edit: " + ex.Message);
                    ModelState.AddModelError("", "Erro ao atualizar.");
                }
            }

            return View(viewModel);
        }

        // =========================
        // DELETE
        // =========================
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            var vm = new FormaPagamentoViewModel
            {
                IdFormaPagamento = item.IdFormaPagamento,
                DescricaoFormaPagamento = item.DescricaoFormaPagamento
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
                Console.WriteLine("[ERRO] FormaPagamento Delete: " + ex.Message);
                ViewData["DeleteError"] = "Não foi possível excluir. Pode estar em uso.";

                var item = await _repo.GetByIdAsync(id);

                var vm = new FormaPagamentoViewModel
                {
                    IdFormaPagamento = item.IdFormaPagamento,
                    DescricaoFormaPagamento = item.DescricaoFormaPagamento
                };

                return View("Delete", vm);
            }
        }
    }
}
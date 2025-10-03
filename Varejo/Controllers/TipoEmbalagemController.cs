using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class TipoEmbalagemController : Controller
    {
        private readonly ITipoEmbalagemRepository _tipoEmbalagemRepository;

        public TipoEmbalagemController(ITipoEmbalagemRepository tipoEmbalagemRepository)
        {
            _tipoEmbalagemRepository = tipoEmbalagemRepository;
        }

        // GET: TipoEmbalagem
        public async Task<IActionResult> Index()
        {
            var tipos = await _tipoEmbalagemRepository.GetAllAsync();
            var viewModels = tipos.Select(t => new TipoEmbalagemViewModel
            {
                IdTipoEmbalagem = t.IdTipoEmbalagem,
                DescricaoTipoEmbalagem = t.DescricaoTipoEmbalagem,
                Multiplicador = t.Multiplicador
            }).ToList();

            return View(viewModels);
        }

        // GET: TipoEmbalagem/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoEmbalagem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoEmbalagemViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var tipo = new TipoEmbalagem
                {
                    DescricaoTipoEmbalagem = viewModel.DescricaoTipoEmbalagem,
                    Multiplicador = viewModel.Multiplicador
                };

                await _tipoEmbalagemRepository.AddAsync(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: TipoEmbalagem/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tipo = await _tipoEmbalagemRepository.GetByIdAsync(id);
            if (tipo == null)
                return NotFound();

            var viewModel = new TipoEmbalagemViewModel
            {
                IdTipoEmbalagem = tipo.IdTipoEmbalagem,
                DescricaoTipoEmbalagem = tipo.DescricaoTipoEmbalagem,
                Multiplicador = tipo.Multiplicador
            };

            return View(viewModel);
        }

        // POST: TipoEmbalagem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoEmbalagemViewModel viewModel)
        {
            if (id != viewModel.IdTipoEmbalagem)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var tipo = new TipoEmbalagem
                {
                    IdTipoEmbalagem = viewModel.IdTipoEmbalagem,
                    DescricaoTipoEmbalagem = viewModel.DescricaoTipoEmbalagem,
                    Multiplicador = viewModel.Multiplicador
                };

                await _tipoEmbalagemRepository.UpdateAsync(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: TipoEmbalagem/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var tipo = await _tipoEmbalagemRepository.GetByIdAsync(id);
            if (tipo == null)
                return NotFound();

            var viewModel = new TipoEmbalagemViewModel
            {
                IdTipoEmbalagem = tipo.IdTipoEmbalagem,
                DescricaoTipoEmbalagem = tipo.DescricaoTipoEmbalagem,
                Multiplicador = tipo.Multiplicador
            };

            return View(viewModel);
        }

        // POST: TipoEmbalagem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _tipoEmbalagemRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

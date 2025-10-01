using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class CategoriaController : Controller
    {

        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaController(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        // GET: Categoria
        public async Task<IActionResult> Index()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            var viewModels = categorias.Select(c => new CategoriaViewModel
            {
                IdCategoria = c.IdCategoria,
                DescricaoCategoria = c.DescricaoCategoria,
                QuantidadeFamilia = c.Familias.Count
            }).ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                return NotFound();

            var viewModel = new CategoriaViewModel
            {
                IdCategoria = categoria.IdCategoria,
                DescricaoCategoria = categoria.DescricaoCategoria,
                QuantidadeFamilia = categoria.Familias.Count,
                Familias = categoria.Familias.Select(f => new FamiliaCategoriaViewModel
                {
                    IdFamilia = f.IdFamilia,
                    NomeFamilia = f.NomeFamilia,
                    Ativo = f.Ativo
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Categoria/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categoria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var categoria = new Categoria
                {
                    DescricaoCategoria = viewModel.DescricaoCategoria
                };

                await _categoriaRepository.AddAsync(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Categoria/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                return NotFound();

            var viewModel = new CategoriaViewModel
            {
                IdCategoria = categoria.IdCategoria,
                DescricaoCategoria = categoria.DescricaoCategoria
            };

            return View(viewModel);
        }

        // POST: Categoria/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriaViewModel viewModel)
        {
            if (id != viewModel.IdCategoria)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var categoria = new Categoria
                {
                    IdCategoria = viewModel.IdCategoria,
                    DescricaoCategoria = viewModel.DescricaoCategoria
                };

                await _categoriaRepository.UpdateAsync(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Categoria/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                return NotFound();

            var viewModel = new CategoriaViewModel
            {
                IdCategoria = categoria.IdCategoria,
                DescricaoCategoria = categoria.DescricaoCategoria,
                QuantidadeFamilia = categoria.Familias.Count
            };

            return View(viewModel);
        }

        // POST: Categoria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoriaRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

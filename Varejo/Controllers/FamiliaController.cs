using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Controllers
{
    public class FamiliaController : Controller
    {
        private readonly IFamiliaRepository _familiaRepository;
        private readonly IMarcaRepository _marcaRepository;
        private readonly ICategoriaRepository _categoriaRepository;


        public FamiliaController(
            IFamiliaRepository familiaRepository, IMarcaRepository marcaRepository, ICategoriaRepository categoriaRepository)
        {
            _familiaRepository = familiaRepository;
            _marcaRepository = marcaRepository;
            _categoriaRepository = categoriaRepository;

        }

        // LISTAR
        [HttpPost, ActionName("Listar")]
        public async Task<IActionResult> Index()
        {
            var familias = await _familiaRepository.GetAllAsync();
            return View(familias);
        }

        // CRIAR
        public async Task<IActionResult> Create()
        {
            ViewBag.Marcas = await _marcaRepository.GetAllAsync();
            return View();
        }

        [HttpPost, ActionName("Criar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Familia familia)
        {
            if (ModelState.IsValid)
            {
                await _familiaRepository.AddAsync(familia);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Marcas = await _marcaRepository.GetAllAsync();
            return View(familia);
        }

        // EDITAR
        public async Task<IActionResult> Edit(int id)
        {
            var familia = await _familiaRepository.GetByIdAsync(id);

            if (familia == null) return NotFound();

            ViewBag.Marcas = await _marcaRepository.GetAllAsync();
            return View(familia);

            
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Familia familia)
        {
            if (id != familia.IdFamilia) return NotFound();

            if (ModelState.IsValid)
            {
                await _familiaRepository.UpdateAsync(familia);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Marcas = await _marcaRepository.GetAllAsync();
            return View(familia);
        }

        // EXCLUIR
        public async Task<IActionResult> Delete(int id)
        {
            var familia = await _familiaRepository.GetByIdAsync(id);
            if (familia == null) return NotFound();
            return View(familia);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _familiaRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

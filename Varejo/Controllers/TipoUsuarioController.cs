using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class TipoUsuarioController : Controller
    {
        private readonly ITipoUsuarioRepository _tipoUsuario;

        public TipoUsuarioController(ITipoUsuarioRepository tipoUsuario)
        {
            _tipoUsuario = tipoUsuario;
        }

        public async Task<IActionResult> Index()
        {
            var tipos = await _tipoUsuario.GetAllAsync();
            return View(tipos);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(TipoUsuario tipos)
        {
            if (ModelState.IsValid)
            {
                await _tipoUsuario.AddAsync(tipos);
                return RedirectToAction(nameof(Index));
            }
            return View(tipos);

        }

        public async Task<IActionResult> Edit(int id)
        {
            var tipo = await _tipoUsuario.GetByIdAsync(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoUsuario tipo)
        {
            if (id != tipo.IdTipoUsuario) return NotFound();

            if (ModelState.IsValid)
            {
                await _tipoUsuario.UpdateAsync(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var tipo = await _tipoUsuario.GetByIdAsync(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _tipoUsuario.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    } 
}

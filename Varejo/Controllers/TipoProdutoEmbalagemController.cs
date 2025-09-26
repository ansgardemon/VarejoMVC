using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using System.Threading.Tasks;

namespace Varejo.Controllers
{
    public class TipoEmbalagemController : Controller
    {
        private readonly ITipoEmbalagemRepository _tipoEmbalagemRepository;

        public TipoEmbalagemController(ITipoEmbalagemRepository tipoEmbalagemRepository)
        {
            _tipoEmbalagemRepository = tipoEmbalagemRepository;
        }

        // LISTAR
        [HttpPost, ActionName("Listar")]
        public async Task<IActionResult> Index()
        {
            var lista = await _tipoEmbalagemRepository.GetAllAsync();
            return View(lista);
        }

        // CRIAR
        public IActionResult Create() => View();

        [HttpPost, ActionName("Criar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoEmbalagem tipoEmbalagem)
        {
            if (ModelState.IsValid)
            {
                await _tipoEmbalagemRepository.AddAsync(tipoEmbalagem);
                return RedirectToAction(nameof(Index));
            }
            return View(tipoEmbalagem);
        }

        // EDITAR
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _tipoEmbalagemRepository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoEmbalagem tipoEmbalagem)
        {
            if (id != tipoEmbalagem.IdTipoEmbalagem) return NotFound();
            if (ModelState.IsValid)
            {
                await _tipoEmbalagemRepository.UpdateAsync(tipoEmbalagem);
                return RedirectToAction(nameof(Index));
            }
            return View(tipoEmbalagem);
        }

        // DELETAR
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _tipoEmbalagemRepository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _tipoEmbalagemRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

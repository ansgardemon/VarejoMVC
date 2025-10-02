using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Controllers
{
    public class ProdutoController: Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IFamiliaRepository _familiaRepository;

        public ProdutoController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;

        }

        // LISTAR
        [HttpPost, ActionName("Listar")]
        public async Task<IActionResult> Index()
        {
            var produtos = await _produtoRepository.GetAllAsync();
            return View(produtos);
        }

        // CRIAR
        public async Task<IActionResult> Create()
        {
            ViewBag.Familias = await _familiaRepository.GetAllAsync(); // exibir opções de família
            return View();
        }

        [HttpPost, ActionName("Criar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto)
        {
            if (ModelState.IsValid)
            {
                await _produtoRepository.AddAsync(produto);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Familias = await _familiaRepository.GetAllAsync();
            return View(produto);
        }

        // EDITAR
        public async Task<IActionResult> Edit(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null) return NotFound();

            ViewBag.Familias = await _familiaRepository.GetAllAsync();
            return View(produto);
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produto produto)
        {
            if (id != produto.IdProduto) return NotFound();

            if (ModelState.IsValid)
            {
                await _produtoRepository.UpdateAsync(produto);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Familias = await _familiaRepository.GetAllAsync();
            return View(produto);
        }

        // EXCLUIR
        public async Task<IActionResult> Delete(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _produtoRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}

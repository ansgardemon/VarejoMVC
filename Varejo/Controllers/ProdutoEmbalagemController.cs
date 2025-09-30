using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.Repositories;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class ProdutoEmbalagemController: Controller
    {
        private readonly IProdutoEmbalagemRepository _produtoEmbalagemRepository;

        public ProdutoEmbalagemController(IProdutoEmbalagemRepository produtoEmbalagemRepository)
        {
            _produtoEmbalagemRepository = produtoEmbalagemRepository;
        }


        [HttpPost, ActionName("Listar")]
        public async Task<IActionResult> Index()
        {
            var lista = await _produtoEmbalagemRepository.GetAllAsync();
            return View(lista);
        }


        public IActionResult Create() => View();

        [HttpPost, ActionName("Criar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProdutoEmbalagem produtoEmbalagem)
        {
            if (ModelState.IsValid)
            {
                await _produtoEmbalagemRepository.AddAsync(produtoEmbalagem);
                return RedirectToAction(nameof(Index));
            }
            return View(produtoEmbalagem);
        }

        // POST: ProdutoEmbalagem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProdutoEmbalagem vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var produtoEmbalagem = await _produtoEmbalagemRepository.GetByIdAsync(vm.IdProdutoEmbalagem);
            if (produtoEmbalagem == null) return NotFound();

            // Mapear ViewModel -> Entidade
            produtoEmbalagem.Preco = vm.Preco;
            produtoEmbalagem.Ean = vm.Ean;
            produtoEmbalagem.ProdutoId = vm.ProdutoId;
            produtoEmbalagem.TipoEmbalagemId = vm.TipoEmbalagemId;

            await _produtoEmbalagemRepository.UpdateAsync(produtoEmbalagem);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _produtoEmbalagemRepository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);

        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IFamiliaRepository _familiaRepository;

        public ProdutoController(IProdutoRepository produtoRepository, IFamiliaRepository familiaRepository)
        {
            _produtoRepository = produtoRepository;
            _familiaRepository = familiaRepository;
        }

        // CREATE GET
        public async Task<IActionResult> Create(int familiaId)
        {
            var familia = await _familiaRepository.GetByIdAsync(familiaId);
            if (familia == null) return NotFound();

            var viewModel = new ProdutoViewModel
            {
                FamiliaId = familiaId,
                Ativo = true
            };

            ViewBag.NomeFamilia = familia.NomeFamilia;
            return View(viewModel);
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProdutoViewModel viewModel)
        {
            var familia = await _familiaRepository.GetByIdAsync(viewModel.FamiliaId);
            if (familia == null) return NotFound();

            // Tratar imagem antes de criar o produto
            string urlImagem;
            if (viewModel.ImagemUpload != null)
            {
                var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.ImagemUpload.FileName);
                var caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", nomeArquivo);

                using var stream = new FileStream(caminho, FileMode.Create);
                await viewModel.ImagemUpload.CopyToAsync(stream);

                urlImagem = "/img/" + nomeArquivo;
            }
            else
            {
                urlImagem = "/img/sem-imagem.png"; // fallback obrigatório
            }

            // NomeProduto automático
            var nomeProduto = $"{familia.NomeFamilia} {viewModel.Complemento}";

            // Criar entidade com todos os campos obrigatórios preenchidos
            var produto = new Produto
            {
                NomeProduto = nomeProduto,
                Complemento = viewModel.Complemento,
                EstoqueInicial = 0,
                Ativo = viewModel.Ativo,
                UrlImagem = urlImagem,
                CustoMedio = 0,
                FamiliaId = viewModel.FamiliaId
            };

            await _produtoRepository.AddAsync(produto);
            return RedirectToAction("Details", "Familia", new { id = viewModel.FamiliaId });
        }

        // DETAILS
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null) return NotFound();

            var produtoVm = new ProdutoViewModel
            {
                IdProduto = produto.IdProduto,
                NomeProduto = produto.NomeProduto,
                Complemento = produto.Complemento,
                EstoqueInicial = produto.EstoqueInicial,
                CustoMedio = produto.CustoMedio,
                Ativo = produto.Ativo,
                UrlImagem = produto.UrlImagem,
                FamiliaId = produto.FamiliaId,
                Familia = produto.Familia
            };

            return View(produtoVm);
        }


        // GET: Produto/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null) return NotFound();

            var produtoVm = new ProdutoViewModel
            {
                IdProduto = produto.IdProduto,
                NomeProduto = produto.NomeProduto,
                Complemento = produto.Complemento,
                FamiliaId = produto.FamiliaId
            };

            return View(produtoVm);
        }

        // POST: Produto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null) return NotFound();

            await _produtoRepository.DeleteAsync(id);
            return RedirectToAction("Details", "Familia", new { id = produto.FamiliaId });
        }




    }
}

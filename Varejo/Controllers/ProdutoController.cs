using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{


    public class ProdutoController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IFamiliaRepository _familiaRepository;
        private readonly ITipoEmbalagemRepository _tipoEmbalagemRepository;

        public ProdutoController(
                    IProdutoRepository produtoRepository,
                    IFamiliaRepository familiaRepository,
                    ITipoEmbalagemRepository tipoEmbalagemRepository)
        {
            _produtoRepository = produtoRepository;
            _familiaRepository = familiaRepository;
            _tipoEmbalagemRepository = tipoEmbalagemRepository;
        }

        // CREATE GET
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Create(int familiaId)
        {


            var familia = await _familiaRepository.GetByIdAsync(familiaId);
            if (familia == null)
            {

                return NotFound();
            }

            var tipos = await _tipoEmbalagemRepository.GetAllAsync();
            if (tipos == null || !tipos.Any())
            {

                tipos = new List<TipoEmbalagem>(); // fallback
            }


            // Inicializa Embalagens garantindo que TiposEmbalagem nunca seja null
            var viewModel = new ProdutoViewModel
            {
                FamiliaId = familiaId,
                Ativo = true,
                Embalagens = new List<ProdutoEmbalagemViewModel>
        {
            new ProdutoEmbalagemViewModel
            {
                TiposEmbalagem = tipos.Select(t => new SelectListItem
                {
                    Value = t.IdTipoEmbalagem.ToString(),
                    Text = t.DescricaoTipoEmbalagem
                }).ToList()
            }
        }
            };

            ViewBag.NomeFamilia = familia.NomeFamilia;
            ViewBag.TiposEmbalagem = tipos;



            return View(viewModel);
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProdutoViewModel viewModel)
        {


            var familia = await _familiaRepository.GetByIdAsync(viewModel.FamiliaId);
            if (familia == null)
            {

                return NotFound();
            }

            var tipos = await _tipoEmbalagemRepository.GetAllAsync() ?? new List<TipoEmbalagem>();



            if (viewModel.Embalagens != null)
            {
                for (int i = 0; i < viewModel.Embalagens.Count; i++)
                {
                    var e = viewModel.Embalagens[i];

                }
            }

            // Garantir que Embalagens nunca seja null
            if (viewModel.Embalagens == null || !viewModel.Embalagens.Any())
            {
                viewModel.Embalagens = new List<ProdutoEmbalagemViewModel>
        {
            new ProdutoEmbalagemViewModel
            {
                TiposEmbalagem = tipos.Select(t => new SelectListItem
                {
                    Value = t.IdTipoEmbalagem.ToString(),
                    Text = t.DescricaoTipoEmbalagem
                }).ToList()
            }
        };
            }
            else
            {
                // Repopular TiposEmbalagem para cada embalagem existente
                foreach (var emb in viewModel.Embalagens)
                {
                    emb.TiposEmbalagem = tipos.Select(t => new SelectListItem
                    {
                        Value = t.IdTipoEmbalagem.ToString(),
                        Text = t.DescricaoTipoEmbalagem
                    }).ToList();
                }

            }

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($" - Campo '{key}': {error.ErrorMessage}");
                    }
                }

                ViewBag.NomeFamilia = familia.NomeFamilia;
                ViewBag.TiposEmbalagem = tipos;
                return View(viewModel);
            }

            // Tratar imagem
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
                urlImagem = "/img/sem-imagem.png";
            }

            // NomeProduto automático
            var nomeProduto = $"{familia.NomeFamilia} {viewModel.Complemento}";

            // Criar entidade Produto
            var produto = new Produto
            {
                NomeProduto = nomeProduto,
                Complemento = viewModel.Complemento,
                EstoqueInicial = 0,
                Ativo = viewModel.Ativo,
                UrlImagem = urlImagem,
                CustoMedio = 0,
                FamiliaId = viewModel.FamiliaId,
                ProdutosEmbalagem = viewModel.Embalagens
                    .Where(e => e != null)
                    .Select(e => new ProdutoEmbalagem
                    {
                        Preco = e.Preco,
                        Ean = e.Ean,
                        TipoEmbalagemId = e.TipoEmbalagemId
                    }).ToList()
            };

            try
            {
                await _produtoRepository.AddAsync(produto);
            }

            catch (Exception ex)
            {
                Console.WriteLine("[ERRO] Ao criar produto: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Não foi possível criar o produto. Verifique se o EAN já existe.");

                // Recarregar listas para ViewBag
                var tipo = await _tipoEmbalagemRepository.GetAllAsync() ?? new List<TipoEmbalagem>();
                viewModel.Embalagens = viewModel.Embalagens ?? new List<ProdutoEmbalagemViewModel>();
                foreach (var emb in viewModel.Embalagens)
                {
                    emb.TiposEmbalagem = tipos.Select(t => new SelectListItem
                    {
                        Value = t.IdTipoEmbalagem.ToString(),
                        Text = t.DescricaoTipoEmbalagem
                    }).ToList();
                }

                ViewBag.NomeFamilia = familia.NomeFamilia;
                ViewBag.TiposEmbalagem = tipos;

                return View(viewModel);
            }

            return RedirectToAction("Details", "Familia", new { id = viewModel.FamiliaId });
        }






        // DETAILS
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null) return NotFound();

            // Mapear para ViewModel incluindo Embalagens
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
                Familia = produto.Familia,
                Embalagens = produto.ProdutosEmbalagem.Select(e => new ProdutoEmbalagemViewModel
                {
                    IdProdutoEmbalagem = e.IdProdutoEmbalagem,
                    Preco = e.Preco,
                    Ean = e.Ean,
                    ProdutoId = e.ProdutoId,
                    TipoEmbalagemId = e.TipoEmbalagemId,
                    TiposEmbalagem = new List<SelectListItem> // só para exibir o nome da embalagem
            {
                new SelectListItem
                {
                    Value = e.TipoEmbalagemId.ToString(),
                    Text = e.TipoEmbalagem?.DescricaoTipoEmbalagem ?? "—"
                }
            }
                }).ToList()
            };

            return View(produtoVm);
        }




        // EDIT GET
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null) return NotFound();

            var tipos = await _tipoEmbalagemRepository.GetAllAsync() ?? new List<TipoEmbalagem>();

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
                Familia = produto.Familia,
                Embalagens = produto.ProdutosEmbalagem.Select(e => new ProdutoEmbalagemViewModel
                {
                    IdProdutoEmbalagem = e.IdProdutoEmbalagem,
                    Preco = e.Preco,
                    Ean = e.Ean,
                    ProdutoId = e.ProdutoId,
                    TipoEmbalagemId = e.TipoEmbalagemId,
                    TiposEmbalagem = tipos.Select(t => new SelectListItem
                    {
                        Value = t.IdTipoEmbalagem.ToString(),
                        Text = t.DescricaoTipoEmbalagem,
                        Selected = t.IdTipoEmbalagem == e.TipoEmbalagemId
                    }).ToList()
                }).ToList()
            };

            ViewBag.NomeFamilia = produto.Familia?.NomeFamilia;
            ViewBag.TiposEmbalagem = tipos;

            return View(produtoVm);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProdutoViewModel viewModel)
        {
            var produto = await _produtoRepository.GetByIdAsync(viewModel.IdProduto);
            if (produto == null) return NotFound();

            var tipos = await _tipoEmbalagemRepository.GetAllAsync() ?? new List<TipoEmbalagem>();

            // Validação
            if (!ModelState.IsValid)
            {
                ViewBag.NomeFamilia = (await _familiaRepository.GetByIdAsync(viewModel.FamiliaId))?.NomeFamilia;
                ViewBag.TiposEmbalagem = tipos;
                // Repopular TiposEmbalagem para cada embalagem existente
                if (viewModel.Embalagens != null)
                {
                    foreach (var emb in viewModel.Embalagens)
                    {
                        emb.TiposEmbalagem = tipos.Select(t => new SelectListItem
                        {
                            Value = t.IdTipoEmbalagem.ToString(),
                            Text = t.DescricaoTipoEmbalagem
                        }).ToList();
                    }
                }
                return View(viewModel);
            }

            // Atualizar dados do produto
            produto.Complemento = viewModel.Complemento;
            produto.Ativo = viewModel.Ativo;

            // Atualizar imagem se houver upload
            if (viewModel.ImagemUpload != null)
            {
                var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.ImagemUpload.FileName);
                var caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", nomeArquivo);

                using var stream = new FileStream(caminho, FileMode.Create);
                await viewModel.ImagemUpload.CopyToAsync(stream);

                produto.UrlImagem = "/img/" + nomeArquivo;
            }

            // NomeProduto automático
            var familia = await _familiaRepository.GetByIdAsync(produto.FamiliaId);
            produto.NomeProduto = $"{familia?.NomeFamilia} {produto.Complemento}";

            // Atualizar embalagens
            var embalagensRecebidas = viewModel.Embalagens ?? new List<ProdutoEmbalagemViewModel>();

            // Remover embalagens que não existem mais
            var idsRecebidos = embalagensRecebidas
                .Where(e => e.IdProdutoEmbalagem != 0)
                .Select(e => e.IdProdutoEmbalagem)
                .ToList();

            var paraRemover = produto.ProdutosEmbalagem
                .Where(e => !idsRecebidos.Contains(e.IdProdutoEmbalagem))
                .ToList();

            foreach (var emb in paraRemover)
                produto.ProdutosEmbalagem.Remove(emb);

            // Atualizar ou adicionar embalagens
            foreach (var embVm in embalagensRecebidas)
            {
                if (embVm.IdProdutoEmbalagem != 0)
                {
                    // Atualiza embalagem existente
                    var emb = produto.ProdutosEmbalagem.FirstOrDefault(e => e.IdProdutoEmbalagem == embVm.IdProdutoEmbalagem);
                    if (emb != null)
                    {
                        emb.Preco = embVm.Preco;
                        emb.Ean = embVm.Ean;
                        emb.TipoEmbalagemId = embVm.TipoEmbalagemId;
                    }
                }
                else
                {
                    // Nova embalagem
                    produto.ProdutosEmbalagem.Add(new ProdutoEmbalagem
                    {
                        Preco = embVm.Preco,
                        Ean = embVm.Ean,
                        TipoEmbalagemId = embVm.TipoEmbalagemId
                    });
                }
            }


            try
            {
                await _produtoRepository.UpdateAsync(produto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERRO] Ao editar produto: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Não foi possível criar o produto. Verifique se o EAN ou Produto já existem.");

                // Recarregar listas para ViewBag
                var tipo = await _tipoEmbalagemRepository.GetAllAsync() ?? new List<TipoEmbalagem>();
                viewModel.Embalagens = viewModel.Embalagens ?? new List<ProdutoEmbalagemViewModel>();
                foreach (var emb in viewModel.Embalagens)
                {
                    emb.TiposEmbalagem = tipos.Select(t => new SelectListItem
                    {
                        Value = t.IdTipoEmbalagem.ToString(),
                        Text = t.DescricaoTipoEmbalagem
                    }).ToList();
                }

                ViewBag.NomeFamilia = familia.NomeFamilia;
                ViewBag.TiposEmbalagem = tipos;

                return View(viewModel);
            }
            return RedirectToAction("Details", "Familia", new { id = produto.FamiliaId });
        }


        // GET: Produto/Delete/5
        [Authorize(Roles = "Administrador, Gerente")]
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

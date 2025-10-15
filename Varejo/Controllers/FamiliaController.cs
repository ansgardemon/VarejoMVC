using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class FamiliaController : Controller
    {
        private readonly IFamiliaRepository _familiaRepository;

        public FamiliaController(IFamiliaRepository familiaRepository)
        {
            _familiaRepository = familiaRepository;
        }





        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Index(int? marcaId, int? categoriaId, string search)
        {
            var familias = await _familiaRepository.GetAllAsync();

            // FILTROS
            if (!string.IsNullOrEmpty(search))
                familias = familias.Where(f => f.NomeFamilia.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (marcaId.HasValue)
                familias = familias.Where(f => f.MarcaId == marcaId.Value).ToList();

            if (categoriaId.HasValue)
                familias = familias.Where(f => f.CategoriaId == categoriaId.Value).ToList();

            // Preencher ViewModel
            var familiasVm = familias.Select(f => new FamiliaViewModel
            {
                IdFamilia = f.IdFamilia,
                NomeFamilia = f.NomeFamilia,
                Ativo = f.Ativo,
                MarcaId = f.MarcaId,
                Marca = f.Marca,
                CategoriaId = f.CategoriaId,
                Categoria = f.Categoria
            }).ToList();

            // ViewBag para filtros — agora como SelectList, não List<SelectListItem>
            ViewBag.Marcas = new SelectList(_familiaRepository.GetMarcas(), "IdMarca", "NomeMarca", marcaId);
            ViewBag.Categorias = new SelectList(_familiaRepository.GetCategorias(), "IdCategoria", "DescricaoCategoria", categoriaId);
            ViewBag.Search = search;

            return View(familiasVm);
        }



        // DETAILS
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Details(int id)
        {
            var familia = await _familiaRepository.GetByIdAsync(id);
            if (familia == null) return NotFound();

            var familiaVm = new FamiliaViewModel
            {
                IdFamilia = familia.IdFamilia,
                NomeFamilia = familia.NomeFamilia,
                Ativo = familia.Ativo,
                MarcaId = familia.MarcaId,
                Marca = familia.Marca,
                CategoriaId = familia.CategoriaId,
                Categoria = familia.Categoria,
                FamiliaDetails = familia.Produtos?.Select(p => new FamiliaDetailViewModel
                {
                    IdProduto = p.IdProduto,
                    NomeProduto = p.NomeProduto,
                    UrlImagem = p.UrlImagem,
                    Ativo = p.Ativo,
                    FamiliaId = p.FamiliaId
                }).ToList()
            };

            return View(familiaVm);
        }

        // CREATE - GET
        [Authorize(Roles = "Administrador, Gerente")]
        public IActionResult Create()
        {
            // ViewBag para dropdowns de Marca e Categoria
            ViewBag.Marcas = new SelectList(_familiaRepository.GetMarcas(), "IdMarca", "NomeMarca");
            ViewBag.Categorias = new SelectList(_familiaRepository.GetCategorias(), "IdCategoria", "DescricaoCategoria");

            return View(new FamiliaViewModel());
        }

        // CREATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FamiliaViewModel familiaVm)
        {
            if (ModelState.IsValid)
            {
                var familia = new Familia
                {
                    NomeFamilia = familiaVm.NomeFamilia,
                    Ativo = familiaVm.Ativo,
                    MarcaId = familiaVm.MarcaId,
                    CategoriaId = familiaVm.CategoriaId
                };

                try
                {
                    await _familiaRepository.AddAsync(familia);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] Ao criar família: " + ex.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível criar a família. Verifique se o nome já existe.");
                }
            }

            // Recarregar dropdowns caso ModelState seja inválido
            ViewBag.Marcas = new SelectList(_familiaRepository.GetMarcas(), "IdMarca", "NomeMarca");
            ViewBag.Categorias = new SelectList(_familiaRepository.GetCategorias(), "IdCategoria", "DescricaoCategoria");

            return View(familiaVm);
        }

        // EDIT - GET
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            var familia = await _familiaRepository.GetByIdAsync(id);
            if (familia == null) return NotFound();

            var familiaVm = new FamiliaViewModel
            {
                IdFamilia = familia.IdFamilia,
                NomeFamilia = familia.NomeFamilia,
                Ativo = familia.Ativo,
                MarcaId = familia.MarcaId,
                CategoriaId = familia.CategoriaId
            };

            ViewBag.Marcas = new SelectList(_familiaRepository.GetMarcas(), "IdMarca", "NomeMarca", familiaVm.MarcaId);
            ViewBag.Categorias = new SelectList(_familiaRepository.GetCategorias(), "IdCategoria", "DescricaoCategoria", familiaVm.CategoriaId);

            return View(familiaVm);
        }

        // EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FamiliaViewModel familiaVm)
        {
            if (ModelState.IsValid)
            {
                var familia = await _familiaRepository.GetByIdAsync(familiaVm.IdFamilia);
                if (familia == null) return NotFound();

                familia.NomeFamilia = familiaVm.NomeFamilia;
                familia.Ativo = familiaVm.Ativo;
                familia.MarcaId = familiaVm.MarcaId;
                familia.CategoriaId = familiaVm.CategoriaId;

                try
                {
                    await _familiaRepository.UpdateAsync(familia);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] Ao editar família: " + ex.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível atualizar a família. Verifique se o nome já existe ou se há produtos associados.");
                }
            }

            // Recarregar dropdowns caso ModelState seja inválido
            ViewBag.Marcas = new SelectList(_familiaRepository.GetMarcas(), "IdMarca", "NomeMarca", familiaVm.MarcaId);
            ViewBag.Categorias = new SelectList(_familiaRepository.GetCategorias(), "IdCategoria", "DescricaoCategoria", familiaVm.CategoriaId);

            return View(familiaVm);
        }

        // DELETE - GET (confirmação)
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var familia = await _familiaRepository.GetByIdAsync(id);
            if (familia == null) return NotFound();

            var familiaVm = new FamiliaViewModel
            {
                IdFamilia = familia.IdFamilia,
                NomeFamilia = familia.NomeFamilia,
                Ativo = familia.Ativo
            };

            return View(familiaVm);
        }

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _familiaRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

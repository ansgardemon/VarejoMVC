using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.Repositories;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class MarcaController : Controller
    {


        private readonly IMarcaRepository _marcaRepository;

        public MarcaController(IMarcaRepository marcaRepository)
        {
            _marcaRepository = marcaRepository;
        }

        // GET: Marca
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Index()
        {
            var marcas = await _marcaRepository.GetAllAsync();
            var viewModels = marcas.Select(c => new MarcaViewModel
            {
                IdMarca = c.IdMarca,
                NomeMarca = c.NomeMarca,
                QuantidadeFamilia = c.Familias.Count
            }).ToList();

            return View(viewModels);
        }

        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Details(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id);
            if (marca == null)
                return NotFound();

            var viewModel = new MarcaViewModel
            {
                IdMarca = marca.IdMarca,
                NomeMarca = marca.NomeMarca,
                QuantidadeFamilia = marca.Familias.Count,
                Familias = marca.Familias.Select(f => new FamiliaCategoriaViewModel
                {
                    IdFamilia = f.IdFamilia,
                    NomeFamilia = f.NomeFamilia,
                    Ativo = f.Ativo
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Marca/Create
        [Authorize(Roles = "Administrador, Gerente")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Marca/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MarcaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var marca = new Marca
                {
                    NomeMarca = viewModel.NomeMarca
                };


                try
                {
                    await _marcaRepository.AddAsync(marca);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] Ao criar Marca: " + ex.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível criar a marca. Verifique se o nome já existe.");
                }




                }
            return View(viewModel);
        }

        // GET: Marca/Edit/5
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id);
            if (marca == null)
                return NotFound();

            var viewModel = new MarcaViewModel
            {
                IdMarca = marca.IdMarca,
                NomeMarca = marca.NomeMarca
            };

            return View(viewModel);
        }

        // POST: Marca/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MarcaViewModel viewModel)
        {
            if (id != viewModel.IdMarca)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var marca = new Marca
                {
                    IdMarca = viewModel.IdMarca,
                    NomeMarca = viewModel.NomeMarca
                };


                try
                {
                    await _marcaRepository.UpdateAsync(marca);
                    return RedirectToAction(nameof(Index));
                }

                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] Ao criar Marca: " + ex.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível atualizar a marca. Verifique se o nome já existe.");
                }

            }
            return View(viewModel);
        }

        // GET: Marca/Delete/5
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id);
            if (marca == null)
                return NotFound();

            var viewModel = new MarcaViewModel
            {
                IdMarca = marca.IdMarca,
                NomeMarca = marca.NomeMarca,
                QuantidadeFamilia = marca.Familias.Count
            };

            return View(viewModel);
        }
        // POST: Marca/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id);
            if (marca == null)
                return NotFound();

            try
            {
                // Se a marca tiver famílias associadas, impede a exclusão
                if (marca.Familias != null && marca.Familias.Count > 0)
                    throw new InvalidOperationException("A marca possui famílias associadas e não pode ser excluída.");

                await _marcaRepository.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao excluir Marca Id={id}: {ex.Message}");

                // Cria o ViewModel para reexibir a tela com o erro
                var viewModel = new MarcaViewModel
                {
                    IdMarca = marca.IdMarca,
                    NomeMarca = marca.NomeMarca,
                    QuantidadeFamilia = marca.Familias?.Count ?? 0
                };

                // Mensagem de erro visível na view
                ViewData["DeleteError"] = "❌ Não foi possível excluir esta marca, pois há famílias associadas.";

                return View("Delete", viewModel);
            }
        }


    }



}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{

    [Authorize(Roles = "Administrador")]
    public class TipoEmbalagemController : Controller
    {
        private readonly ITipoEmbalagemRepository _tipoEmbalagemRepository;

        public TipoEmbalagemController(ITipoEmbalagemRepository tipoEmbalagemRepository)
        {
            _tipoEmbalagemRepository = tipoEmbalagemRepository;
        }

        // GET: TipoEmbalagem
        public async Task<IActionResult> Index()
        {
            var tipos = await _tipoEmbalagemRepository.GetAllAsync();
            var viewModels = tipos.Select(t => new TipoEmbalagemViewModel
            {
                IdTipoEmbalagem = t.IdTipoEmbalagem,
                DescricaoTipoEmbalagem = t.DescricaoTipoEmbalagem,
                Multiplicador = t.Multiplicador
            }).ToList();

            return View(viewModels);
        }

        // GET: TipoEmbalagem/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoEmbalagem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoEmbalagemViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var tipo = new TipoEmbalagem
                {
                    DescricaoTipoEmbalagem = viewModel.DescricaoTipoEmbalagem,
                    Multiplicador = viewModel.Multiplicador
                };

                await _tipoEmbalagemRepository.AddAsync(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: TipoEmbalagem/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tipo = await _tipoEmbalagemRepository.GetByIdAsync(id);
            if (tipo == null)
                return NotFound();

            var viewModel = new TipoEmbalagemViewModel
            {
                IdTipoEmbalagem = tipo.IdTipoEmbalagem,
                DescricaoTipoEmbalagem = tipo.DescricaoTipoEmbalagem,
                Multiplicador = tipo.Multiplicador
            };

            return View(viewModel);
        }

        // POST: TipoEmbalagem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoEmbalagemViewModel viewModel)
        {
            if (id != viewModel.IdTipoEmbalagem)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var tipo = new TipoEmbalagem
                {
                    IdTipoEmbalagem = viewModel.IdTipoEmbalagem,
                    DescricaoTipoEmbalagem = viewModel.DescricaoTipoEmbalagem,
                    Multiplicador = viewModel.Multiplicador
                };

                await _tipoEmbalagemRepository.UpdateAsync(tipo);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Detecta violação de índice único (chave duplicada)
                if (ex.InnerException is SqlException sqlEx &&
                    sqlEx.Message.Contains("IX_TiposEmbalagem_DescricaoTipoEmbalagem"))
                {
                    ViewData["EditError"] = "Já existe um tipo de embalagem com essa descrição.";
                }
                else
                {
                    ViewData["EditError"] = "Não foi possível salvar as alterações. Verifique os dados e tente novamente.";
                }

                Console.WriteLine($"[ERRO] Falha ao editar TipoEmbalagem Id={id}: {ex.Message}");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewData["EditError"] = "Ocorreu um erro inesperado ao salvar as alterações.";
                Console.WriteLine($"[ERRO GERAL] Falha ao editar TipoEmbalagem Id={id}: {ex.Message}");
                return View(viewModel);
            }
        }


        // GET: TipoEmbalagem/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var tipo = await _tipoEmbalagemRepository.GetByIdAsync(id);
            if (tipo == null)
                return NotFound();

            var viewModel = new TipoEmbalagemViewModel
            {
                IdTipoEmbalagem = tipo.IdTipoEmbalagem,
                DescricaoTipoEmbalagem = tipo.DescricaoTipoEmbalagem,
                Multiplicador = tipo.Multiplicador
            };

            return View(viewModel);
        }

        // POST: TipoEmbalagem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoEmbalagem = await _tipoEmbalagemRepository.GetByIdAsync(id);
            if (tipoEmbalagem == null)
                return NotFound();

            try
            {
                await _tipoEmbalagemRepository.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao excluir Tipo de Embalagem Id={id}: {ex.Message}");

                // Cria a ViewModel para voltar à view de exclusão
                var viewModel = new TipoEmbalagemViewModel
                {
                    IdTipoEmbalagem = tipoEmbalagem.IdTipoEmbalagem,
                    DescricaoTipoEmbalagem = tipoEmbalagem.DescricaoTipoEmbalagem,
                    Multiplicador = tipoEmbalagem.Multiplicador
                };

                // Mensagem de erro para exibir na view
                ViewData["DeleteError"] = "Não foi possível excluir o Tipo de Embalagem. Ele pode estar sendo usado em algum registro.";

                return View("Delete", viewModel);
            }
        }


    }
}

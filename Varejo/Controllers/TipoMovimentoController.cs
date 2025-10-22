using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    [Authorize(Roles = "Administrador, Gerente")]
    public class TipoMovimentoController : Controller
    {
        private readonly ITipoMovimentoRepository _tipoMovimentoRepository;

        public TipoMovimentoController(ITipoMovimentoRepository tipoMovimentoRepository)
        {
            _tipoMovimentoRepository = tipoMovimentoRepository;
        }

        // GET: TipoMovimento
        public async Task<IActionResult> Index()
        {
            var tipos = await _tipoMovimentoRepository.GetAllAsync();
            var viewModels = tipos.Select(t => new TipoMovimentoViewModel
            {
                IdTipoMovimento = t.IdTipoMovimento,
                DescricaoTipoMovimento = t.DescricaoTipoMovimento,
                DescricaoEspecie = t.EspecieMovimento.DescricaoEspecieMovimento
            }).ToList();

            return View(viewModels);
        }


        // GET: TipoMovimento/Create
        public async Task<IActionResult> Create()
        {
            var especies = await _tipoMovimentoRepository.GetAllEspeciesAsync();
            Console.WriteLine("[DEBUG] GET Create - Especies encontradas: " + especies.Count);
            foreach (var e in especies)
            {
                Console.WriteLine($"[DEBUG] Especie: Id={e.IdEspecieMovimento}, Desc={e.DescricaoEspecieMovimento}");
            }

            ViewBag.Especies = especies;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoMovimentoViewModel viewModel)
        {
            Console.WriteLine("[DEBUG] POST Create - ModelState.IsValid: " + ModelState.IsValid);
            Console.WriteLine($"[DEBUG] POST Create - viewModel: Descricao='{viewModel.DescricaoTipoMovimento}', IdEspecieMovimento={viewModel.IdEspecieMovimento}");

            if (ModelState.IsValid)
            {
                var tipo = new TipoMovimento
                {
                    DescricaoTipoMovimento = viewModel.DescricaoTipoMovimento,
                    EspecieMovimentoId = viewModel.IdEspecieMovimento
                };

                try
                {
                    await _tipoMovimentoRepository.AddAsync(tipo);
                    Console.WriteLine("[DEBUG] TipoMovimento adicionado com sucesso!");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Não foi possível adicionar o tipo de movimento. Verifique se já existe.");
                    Console.WriteLine("[ERRO] TipoMovimento Create: " + ex.Message);
                }
            }
            else
            {
                foreach (var erro in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("[ERRO] ModelState: " + erro.ErrorMessage);
                }
            }

            // Re-popula ViewBag para o select
            var especies = await _tipoMovimentoRepository.GetAllEspeciesAsync();
            Console.WriteLine("[DEBUG] Re-populando ViewBag.Especies, total: " + especies.Count);
            ViewBag.Especies = especies;

            return View(viewModel);
        }



        // GET: TipoMovimento/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tipo = await _tipoMovimentoRepository.GetByIdAsync(id);
            if (tipo == null)
                return NotFound();

            var viewModel = new TipoMovimentoViewModel
            {
                IdTipoMovimento = tipo.IdTipoMovimento,
                DescricaoTipoMovimento = tipo.DescricaoTipoMovimento
                // Não incluí IdEspecieMovimento nem DescricaoEspecie para impedir alteração
            };

            return View(viewModel);
        }

        // POST: TipoMovimento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoMovimentoViewModel viewModel)
        {
            if (id != viewModel.IdTipoMovimento)
                return BadRequest();

            if (ModelState.IsValid)
            {
                // Busca o tipo original para manter a espécie
                var tipoOriginal = await _tipoMovimentoRepository.GetByIdAsync(id);
                if (tipoOriginal == null)
                    return NotFound();

                tipoOriginal.DescricaoTipoMovimento = viewModel.DescricaoTipoMovimento;

                try
                {
                    await _tipoMovimentoRepository.UpdateAsync(tipoOriginal);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Não foi possível atualizar o tipo de movimento.");
                    Console.WriteLine("[ERRO] TipoMovimento Edit: " + ex.Message);
                }
            }

            return View(viewModel);
        }
    }
}

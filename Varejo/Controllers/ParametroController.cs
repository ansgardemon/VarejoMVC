using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class ParametroController : Controller
    {
        private readonly IParametroRepository _repository;
        private readonly ITipoMovimentoRepository _tipoRepository;

        public ParametroController(
            IParametroRepository repository,
            ITipoMovimentoRepository tipoRepository)
        {
            _repository = repository;
            _tipoRepository = tipoRepository;
        }

        public async Task<IActionResult> Index()
        {
            var parametro = await _repository.GetAsync();

            var vm = new ParametroViewModel();

            if (parametro != null)
            {
                vm.TipoMovimentoVendaId = parametro.TipoMovimentoVendaId;
                vm.TipoMovimentoCompraId = parametro.TipoMovimentoCompraId;
                vm.TipoMovimentoAvariaId = parametro.TipoMovimentoAvariaId;
                vm.TipoMovimentoEntradaBonificacaoId = parametro.TipoMovimentoEntradaBonificacaoId;
            }

            ViewBag.Tipos = await _tipoRepository.GetAllAsync();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ParametroViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Tipos = await _tipoRepository.GetAllAsync();
                return View(vm);
            }

            var model = new Parametro
            {
                TipoMovimentoVendaId = vm.TipoMovimentoVendaId,
                TipoMovimentoCompraId = vm.TipoMovimentoCompraId,
                TipoMovimentoAvariaId = vm.TipoMovimentoAvariaId,
                TipoMovimentoEntradaBonificacaoId = vm.TipoMovimentoEntradaBonificacaoId

            };

            await _repository.SaveAsync(model);

            TempData["Sucesso"] = "Salvo com sucesso";
            return RedirectToAction(nameof(Index));
        }
    }
}
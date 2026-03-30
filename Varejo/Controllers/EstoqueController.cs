using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class EstoqueController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;

        public EstoqueController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<IActionResult> Index(EstoqueFiltroViewModel filtro)
        {
            var estoque = await _produtoRepository.GetEstoqueAsync(filtro);

            ViewBag.Filtro = filtro;

            return View(estoque);
        }
    }
}
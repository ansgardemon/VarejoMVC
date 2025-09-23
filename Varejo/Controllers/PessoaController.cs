using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.Repositories;

namespace Varejo.Controllers
{
    public class PessoaController : ControllerBase
    {
        private readonly IPessoaRepository _pessoaRepository;

        public PessoaController(IPessoaRepository pessoaRepository)
        {
            _pessoaRepository = pessoaRepository;
        }

        // == CRUD ==

        //READ
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var pessoas = await _pessoaRepository.GetAllAsync();
            pessoas = pessoas.OrderBy(d => d.NomeRazao).ToList();

            return View(pessoas);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
    }
}


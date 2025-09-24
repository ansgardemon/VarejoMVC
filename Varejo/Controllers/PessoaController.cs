using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;


namespace Varejo.Controllers;

public class PessoaController : Controller
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


}



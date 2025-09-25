using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.Repositories;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class PessoaController : Controller
    {
        private readonly IPessoaRepository _pessoaRepository;

        public PessoaController(IPessoaRepository pessoaRepository)
        {
            _pessoaRepository = pessoaRepository;
        }

        // == CRUD ==

        //CREATE
        public async Task<IActionResult> Create() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pessoa pessoa)
        {
            if (ModelState.IsValid)
            {
                await _pessoaRepository.AddAsync(pessoa);
                return RedirectToAction(nameof(Index));
            }
            return View(pessoa);
        }

        //READ
        //[Authorize]
        public async Task<IActionResult> Index()
        {
            var pessoas = await _pessoaRepository.GetAllAsync();
            var pessoasVm = pessoas.OrderBy(p => p.NomeRazao).Select(p => new PessoaViewModel
            {
                IdPessoa = p.IdPessoa,
                NomeRazao = p.NomeRazao,
                TratamentoFantasia = p.TratamentoFantasia,
                CpfCnpj = p.CpfCnpj,
                Ddd = p.Ddd,
                Telefone = p.Telefone,
                Email = p.Email,
                EhJuridico = p.EhJuridico,
                EhUsuario = p.EhUsuario,
                EhCliente = p.EhCliente,
                EhFornecedor = p.EhFornecedor,
                Ativo = p.Ativo
            }).ToList();
            return View(pessoasVm);
        }

        //UPDATE
        public async Task<IActionResult> Edit(int id)
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(id);
            if (pessoa == null) return NotFound();
            return View(pessoa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pessoa pessoa)
        {
            if (id != pessoa.IdPessoa) return NotFound();
            if (ModelState.IsValid)
            {
                await _pessoaRepository.UpdateAsync(pessoa);
                return RedirectToAction(nameof(Index));
            }
            return View(pessoa);
        }


    }
}

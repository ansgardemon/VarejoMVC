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


        //CREATE READ

        public async  Task<IActionResult> Create()
        {
            // Retorna o formulário em branco
            var vm = new PessoaViewModel
            {
                Enderecos = new List<EnderecoViewModel> { new EnderecoViewModel() }
            };

            return View(vm);
        }




        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PessoaViewModel pessoaVm)
        {
            if (ModelState.IsValid)
            {
                var pessoa = new Pessoa
                {
                    NomeRazao = pessoaVm.NomeRazao,
                    TratamentoFantasia = pessoaVm.TratamentoFantasia,
                    CpfCnpj = pessoaVm.CpfCnpj,
                    Ddd = pessoaVm.Ddd,
                    Telefone = pessoaVm.Telefone,
                    Email = pessoaVm.Email,
                    EhJuridico = pessoaVm.EhJuridico,
                    EhUsuario = pessoaVm.EhUsuario,
                    EhCliente = pessoaVm.EhCliente,
                    EhFornecedor = pessoaVm.EhFornecedor,
                    Ativo = pessoaVm.Ativo,

                    Enderecos = pessoaVm.Enderecos?.Select(e => new Endereco
                    {
                        Logradouro = e.Logradouro,
                        Numero = e.Numero,
                        Cep = e.Cep,
                        Bairro = e.Bairro,
                        Cidade = e.Cidade,
                        Uf = e.Uf,
                        Complemento = e.Complemento
                    }).ToList()
                };

                await _pessoaRepository.AddAsync(pessoa);
                return RedirectToAction(nameof(Index));
            }

            return View(pessoaVm);
        }


        //READ INDEX
        //[Authorize]
        //DEIXEI ENXUTO, POIS OS MAIORES DADOS VÃO PRO DETAILS
        public async Task<IActionResult> Index()
        {
            var pessoas = await _pessoaRepository.GetAllAsync();

            var pessoasVm = pessoas.Select(p => new PessoaViewModel
            {
                IdPessoa = p.IdPessoa,
                NomeRazao = p.NomeRazao,
                CpfCnpj = p.CpfCnpj,
                Telefone = p.Telefone,
                Email = p.Email,
                Ativo = p.Ativo
            }).ToList();

            return View(pessoasVm);
        }



        public async Task<IActionResult> Details(int id)
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(id);

            if (pessoa == null) return NotFound();

            var pessoaVm = new PessoaViewModel
            {
                IdPessoa = pessoa.IdPessoa,
                NomeRazao = pessoa.NomeRazao,
                TratamentoFantasia = pessoa.TratamentoFantasia,
                CpfCnpj = pessoa.CpfCnpj,
                Ddd = pessoa.Ddd,
                Telefone = pessoa.Telefone,
                Email = pessoa.Email,
                EhJuridico = pessoa.EhJuridico,
                EhUsuario = pessoa.EhUsuario,
                EhCliente = pessoa.EhCliente,
                EhFornecedor = pessoa.EhFornecedor,
                Ativo = pessoa.Ativo,
                Enderecos = pessoa.Enderecos?.Select(e => new EnderecoViewModel
                {
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Cep = e.Cep,
                    Bairro = e.Bairro,
                    Cidade = e.Cidade,
                    Uf = e.Uf,
                    Complemento = e.Complemento
                }).ToList()
            };

            return View(pessoaVm);
        }



        //UPDATE
        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(id);
            if (pessoa == null) return NotFound();

            // Mapear Pessoa → PessoaViewModel
            var pessoaVm = new PessoaViewModel
            {
                IdPessoa = pessoa.IdPessoa,
                NomeRazao = pessoa.NomeRazao,
                TratamentoFantasia = pessoa.TratamentoFantasia,
                CpfCnpj = pessoa.CpfCnpj,
                Ddd = pessoa.Ddd,
                Telefone = pessoa.Telefone,
                Email = pessoa.Email,
                EhJuridico = pessoa.EhJuridico,
                EhUsuario = pessoa.EhUsuario,
                EhCliente = pessoa.EhCliente,
                EhFornecedor = pessoa.EhFornecedor,
                Ativo = pessoa.Ativo
            };

            return View(pessoaVm);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PessoaViewModel pessoaVm)
        {
            if (id != pessoaVm.IdPessoa) return NotFound();

            if (ModelState.IsValid)
            {
                // Mapear PessoaViewModel → Pessoa
                var pessoa = new Pessoa
                {
                    IdPessoa = pessoaVm.IdPessoa,
                    NomeRazao = pessoaVm.NomeRazao,
                    TratamentoFantasia = pessoaVm.TratamentoFantasia,
                    CpfCnpj = pessoaVm.CpfCnpj,
                    Ddd = pessoaVm.Ddd,
                    Telefone = pessoaVm.Telefone,
                    Email = pessoaVm.Email,
                    EhJuridico = pessoaVm.EhJuridico,
                    EhUsuario = pessoaVm.EhUsuario,
                    EhCliente = pessoaVm.EhCliente,
                    EhFornecedor = pessoaVm.EhFornecedor,
                    Ativo = pessoaVm.Ativo
                };

                await _pessoaRepository.UpdateAsync(pessoa);
                return RedirectToAction(nameof(Index));
            }

            return View(pessoaVm);
        }
    }
}
    


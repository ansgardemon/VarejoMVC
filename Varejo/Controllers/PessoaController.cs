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


                try
                {
                    await _pessoaRepository.AddAsync(pessoa);
                    return RedirectToAction(nameof(Index));
                }

                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] Ao criar pessoa: " + ex.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível criar a pessoa. Verifique se o cpf/cnpj já existe.");
                }


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
                Ddd = p.Ddd,
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
                Ativo = pessoa.Ativo,

                // Mapear Enderecos da entidade para o ViewModel
                Enderecos = pessoa.Enderecos?.Select(e => new EnderecoViewModel
                {
                    IdEndereco = e.IdEndereco, // adiciona isso
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Cep = e.Cep,
                    Bairro = e.Bairro,
                    Cidade = e.Cidade,
                    Uf = e.Uf,
                    Complemento = e.Complemento
                }).ToList() ?? new List<EnderecoViewModel>()
            };

            return View(pessoaVm);
        }



        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PessoaViewModel pessoaVm)
        {
            if (pessoaVm == null) return NotFound();

            int id = pessoaVm.IdPessoa;

            var pessoa = await _pessoaRepository.GetByIdAsync(id);
            if (pessoa == null) return NotFound();

            if (ModelState.IsValid)
            {
                pessoa.NomeRazao = pessoaVm.NomeRazao;
                pessoa.TratamentoFantasia = pessoaVm.TratamentoFantasia;
                pessoa.CpfCnpj = pessoaVm.CpfCnpj;
                pessoa.Ddd = pessoaVm.Ddd;
                pessoa.Telefone = pessoaVm.Telefone;
                pessoa.Email = pessoaVm.Email;
                pessoa.EhJuridico = pessoaVm.EhJuridico;
                pessoa.EhUsuario = pessoaVm.EhUsuario;
                pessoa.EhCliente = pessoaVm.EhCliente;
                pessoa.EhFornecedor = pessoaVm.EhFornecedor;
                pessoa.Ativo = pessoaVm.Ativo;

                pessoa.Enderecos = pessoaVm.Enderecos?.Select(evm => new Endereco
                {
                    IdEndereco = evm.IdEndereco,
                    Logradouro = evm.Logradouro,
                    Numero = evm.Numero,
                    Cep = evm.Cep,
                    Bairro = evm.Bairro,
                    Cidade = evm.Cidade,
                    Uf = evm.Uf,
                    Complemento = evm.Complemento,
                    PessoaId = pessoa.IdPessoa
                }).ToList();


                try
                {
                    await _pessoaRepository.UpdateAsync(pessoa);
                    return RedirectToAction(nameof(Index));
                }

                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] Ao criar pessoa: " + ex.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível criar a pessoa. Verifique se o cpf já existe.");
                }
            }

            return View(pessoaVm);
        }

    }
}
    


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO;

namespace VarejoSHARED.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PessoaController : ControllerBase
    {
       
            private readonly IPessoaRepository _pessoaRepository;

            public PessoaController(IPessoaRepository pessoaRepository)
            {
                _pessoaRepository = pessoaRepository;
            }


            [HttpGet]
            public async Task<ActionResult<IEnumerable<PessoaOutputDTO>>> GetAll()
            {
                var pessoas = await _pessoaRepository.GetAllAsync();

                var result = pessoas.Select(p => new PessoaOutputDTO
                {
                    IdPessoa = p.IdPessoa,
                    NomeRazao = p.NomeRazao,
                    CpfCnpj = p.CpfCnpj,
                    Telefone = p.Telefone,
                    Email = p.Email,
                    Ativo = p.Ativo
                });

                return Ok(result);
            }

            // 🔹 GET: api/pessoa/5
            [HttpGet("{id}")]
            public async Task<ActionResult<PessoaOutputDTO>> GetById(int id)
            {
                var pessoa = await _pessoaRepository.GetByIdAsync(id);

                if (pessoa == null)
                    return NotFound();

                var dto = new PessoaOutputDTO
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

                return Ok(dto);
            }

     
            [HttpPost]
            public async Task<ActionResult> Create(PessoaInputDTO dto)
            {
                var pessoa = new Pessoa
                {
                    NomeRazao = dto.NomeRazao,
                    TratamentoFantasia = dto.TratamentoFantasia,
                    CpfCnpj = dto.CpfCnpj,
                    Ddd = dto.Ddd,
                    Telefone = dto.Telefone,
                    Email = dto.Email,
                    EhJuridico = dto.EhJuridico,
                    EhUsuario = dto.EhUsuario,
                    EhCliente = dto.EhCliente,
                    EhFornecedor = dto.EhFornecedor
                };

                await _pessoaRepository.AddAsync(pessoa);

                return CreatedAtAction(nameof(GetById), new { id = pessoa.IdPessoa }, pessoa.IdPessoa);
            }

     
            [HttpPut("{id}")]
            public async Task<ActionResult> Update(int id, PessoaInputDTO dto)
            {
                var pessoa = await _pessoaRepository.GetByIdAsync(id);

                if (pessoa == null)
                    return NotFound();

                pessoa.NomeRazao = dto.NomeRazao;
                pessoa.TratamentoFantasia = dto.TratamentoFantasia;
                pessoa.CpfCnpj = dto.CpfCnpj;
                pessoa.Ddd = dto.Ddd;
                pessoa.Telefone = dto.Telefone;
                pessoa.Email = dto.Email;
                pessoa.EhJuridico = dto.EhJuridico;
                pessoa.EhUsuario = dto.EhUsuario;
                pessoa.EhCliente = dto.EhCliente;
                pessoa.EhFornecedor = dto.EhFornecedor;

                await _pessoaRepository.UpdateAsync(pessoa);

                return NoContent();
            }

      
     //Pé
            [HttpGet("{id}/enderecos")]
            public async Task<ActionResult> GetEnderecos(int id)///k,kokok
            {
                var pessoa = await _pessoaRepository.GetByIdAsync(id);

                if (pessoa == null)
                    return NotFound();

                var enderecos = pessoa.Enderecos?.Select(e => new
                {
                    e.IdEndereco,
                    e.Logradouro,
                    e.Numero,
                    e.Cep,
                    e.Bairro,
                    e.Cidade,
                    e.Uf,
                    e.Complemento
                });

                return Ok(enderecos);
            }
        }
    }



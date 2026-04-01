using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO;

namespace VarejoSHARED.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnderecoController : ControllerBase
    {
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IPessoaRepository _pessoaRepository;

        public EnderecoController(
            IEnderecoRepository enderecoRepository,
            IPessoaRepository pessoaRepository)
        {
            _enderecoRepository = enderecoRepository;
            _pessoaRepository = pessoaRepository;
        }

        // 🔹 GET: api/endereco
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnderecoOutputDTO>>> GetAll()
        {
            var enderecos = await _enderecoRepository.GetAllAsync();

            var result = enderecos.Select(e => new EnderecoOutputDTO
            {
                IdEndereco = e.IdEndereco,
                Logradouro = e.Logradouro,
                Cep = e.Cep,
                Bairro = e.Bairro,
                Cidade = e.Cidade,
                Uf = e.Uf,
                Complemento = e.Complemento,
                Numero = e.Numero,
                PessoaId = e.PessoaId
            });

            return Ok(result);
        }

        // 🔹 GET: api/endereco/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EnderecoOutputDTO>> GetById(int id)
        {
            var e = await _enderecoRepository.GetByIdAsync(id);

            if (e == null)
                return NotFound();

            return Ok(new EnderecoOutputDTO
            {
                IdEndereco = e.IdEndereco,
                Logradouro = e.Logradouro,
                Cep = e.Cep,
                Bairro = e.Bairro,
                Cidade = e.Cidade,
                Uf = e.Uf,
                Complemento = e.Complemento,
                Numero = e.Numero,
                PessoaId = e.PessoaId
            });
        }

        // 🔥 NOVO: GET por pessoa (ESSENCIAL pro Blazor)
        // GET: api/endereco/pessoa/1
        [HttpGet("pessoa/{pessoaId}")]
        public async Task<ActionResult<IEnumerable<EnderecoOutputDTO>>> GetByPessoa(int pessoaId)
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(pessoaId);
            if (pessoa == null)
                return NotFound("Pessoa não encontrada.");

            var enderecos = pessoa.Enderecos?.Select(e => new EnderecoOutputDTO
            {
                IdEndereco = e.IdEndereco,
                Logradouro = e.Logradouro,
                Cep = e.Cep,
                Bairro = e.Bairro,
                Cidade = e.Cidade,
                Uf = e.Uf,
                Complemento = e.Complemento,
                Numero = e.Numero,
                PessoaId = e.PessoaId
            });

            return Ok(enderecos);
        }

        // 🔥 MELHORADO: POST vinculado à pessoa
        // POST: api/endereco/pessoa/1
        [HttpPost("pessoa/{pessoaId}")]
        public async Task<ActionResult> Create(int pessoaId, [FromBody] EnderecoInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pessoa = await _pessoaRepository.GetByIdAsync(pessoaId);
            if (pessoa == null)
                return NotFound("Pessoa não encontrada.");

            var endereco = new Endereco
            {
                Logradouro = dto.Logradouro,
                Cep = dto.Cep,
                Bairro = dto.Bairro,
                Cidade = dto.Cidade,
                Uf = dto.Uf,
                Complemento = dto.Complemento,
                Numero = dto.Numero,
                PessoaId = pessoaId
            };

            await _enderecoRepository.AddAsync(endereco);

            return CreatedAtAction(nameof(GetById),
                new { id = endereco.IdEndereco },
                endereco.IdEndereco);
        }

        // 🔹 PUT: api/endereco/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] EnderecoInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var endereco = await _enderecoRepository.GetByIdAsync(id);

            if (endereco == null)
                return NotFound();

            // ❌ NÃO deixa trocar PessoaId
            endereco.Logradouro = dto.Logradouro;
            endereco.Cep = dto.Cep;
            endereco.Bairro = dto.Bairro;
            endereco.Cidade = dto.Cidade;
            endereco.Uf = dto.Uf;
            endereco.Complemento = dto.Complemento;
            endereco.Numero = dto.Numero;

            await _enderecoRepository.UpdateAsync(endereco);

            return NoContent();
        }

        // 🔹 DELETE: api/endereco/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var endereco = await _enderecoRepository.GetByIdAsync(id);

            if (endereco == null)
                return NotFound();

            await _enderecoRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}
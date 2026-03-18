using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoAPI.DTO;

namespace VarejoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnderecoController : ControllerBase
    {
        private readonly IEnderecoRepository _enderecoRepository;

        public EnderecoController(IEnderecoRepository enderecoRepository)
        {
            _enderecoRepository = enderecoRepository;
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

            var dto = new EnderecoOutputDTO
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
            };

            return Ok(dto);
        }

        // 🔹 POST: api/endereco
        [HttpPost]
        public async Task<ActionResult> Create(EnderecoInputDTO dto)
        {
            var endereco = new Endereco
            {
                Logradouro = dto.Logradouro,
                Cep = dto.Cep,
                Bairro = dto.Bairro,
                Cidade = dto.Cidade,
                Uf = dto.Uf,
                Complemento = dto.Complemento,
                Numero = dto.Numero,
                PessoaId = dto.PessoaId
            };

            await _enderecoRepository.AddAsync(endereco);

            return CreatedAtAction(nameof(GetById), new { id = endereco.IdEndereco }, endereco.IdEndereco);
        }

        // 🔹 PUT: api/endereco/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, EnderecoInputDTO dto)
        {
            var endereco = await _enderecoRepository.GetByIdAsync(id);

            if (endereco == null)
                return NotFound();

            endereco.Logradouro = dto.Logradouro;
            endereco.Cep = dto.Cep;
            endereco.Bairro = dto.Bairro;
            endereco.Cidade = dto.Cidade;
            endereco.Uf = dto.Uf;
            endereco.Complemento = dto.Complemento;
            endereco.Numero = dto.Numero;
            endereco.PessoaId = dto.PessoaId;

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

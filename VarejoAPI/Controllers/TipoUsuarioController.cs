using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace VarejoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoUsuarioController : ControllerBase
    {
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;

        public TipoUsuarioController(ITipoUsuarioRepository tipoUsuarioRepository)
        {
            _tipoUsuarioRepository = tipoUsuarioRepository;
        }

        // 🔹 GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoUsuarioOutputDTO>>> GetAll()
        {
            var tipos = await _tipoUsuarioRepository.GetAllAsync();

            // Mapeando a lista para o DTO de saída
            var output = tipos.Select(t => new TipoUsuarioOutputDTO
            {
                IdTipoUsuario = t.IdTipoUsuario,
                DescricaoTipoUsuario = t.DescricaoTipoUsuario
            });

            return Ok(output);
        }

        // 🔹 GET por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoUsuarioOutputDTO>> Get(int id)
        {
            var tipoUsuario = await _tipoUsuarioRepository.GetByIdAsync(id);

            if (tipoUsuario == null)
                return NotFound(new { message = "Tipo de usuário não encontrado." });

            var output = new TipoUsuarioOutputDTO
            {
                IdTipoUsuario = tipoUsuario.IdTipoUsuario,
                DescricaoTipoUsuario = tipoUsuario.DescricaoTipoUsuario
            };

            return Ok(output);
        }

        // 🔹 POST (Create)
        [HttpPost]
        public async Task<ActionResult<TipoUsuarioOutputDTO>> Post([FromBody] TipoUsuarioInputDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.DescricaoTipoUsuario))
                return BadRequest("Dados inválidos.");

            var tipoUsuario = new TipoUsuario
            {
                DescricaoTipoUsuario = dto.DescricaoTipoUsuario
            };

            await _tipoUsuarioRepository.AddAsync(tipoUsuario);

            // Mapeia para o Output para retornar o ID gerado
            var output = new TipoUsuarioOutputDTO
            {
                IdTipoUsuario = tipoUsuario.IdTipoUsuario,
                DescricaoTipoUsuario = tipoUsuario.DescricaoTipoUsuario
            };

            return CreatedAtAction(nameof(Get), new { id = output.IdTipoUsuario }, output);
        }

        // 🔹 PUT (Update)
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] TipoUsuarioInputDTO dto)
        {
            var tipoUsuario = await _tipoUsuarioRepository.GetByIdAsync(id);

            if (tipoUsuario == null)
                return NotFound();

            tipoUsuario.DescricaoTipoUsuario = dto.DescricaoTipoUsuario;

            await _tipoUsuarioRepository.UpdateAsync(tipoUsuario);

            return NoContent();
        }

        // 🔹 DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var tipoUsuario = await _tipoUsuarioRepository.GetByIdAsync(id);

            if (tipoUsuario == null)
                return NotFound();

            await _tipoUsuarioRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}
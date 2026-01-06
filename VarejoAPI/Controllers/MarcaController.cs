using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoAPI.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarcaController : ControllerBase
    {
        private readonly IMarcaRepository _marcaRepository;

        public MarcaController(IMarcaRepository marcaRepository)
        {
            _marcaRepository = marcaRepository;
        }

        // GET: api/Marca
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarcaOutputDTO>>> Get()
        {
            var marcas = await _marcaRepository.GetAllAsync();

            var resultado = marcas.Select(m => new MarcaOutputDTO
            {
                IdMarca = m.IdMarca,
                NomeMarca = m.NomeMarca
            });

            return Ok(resultado);
        }

        // GET: api/Marca/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MarcaOutputDTO>> Get(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id);

            if (marca == null)
                return NotFound();

            var resultado = new MarcaOutputDTO
            {
                IdMarca = marca.IdMarca,
                NomeMarca = marca.NomeMarca
            };

            return Ok(resultado);
        }

        // POST: api/Marca
        [HttpPost]
        public async Task<ActionResult<MarcaOutputDTO>> Post([FromBody] MarcaInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var marca = new Marca
            {
                NomeMarca = dto.NomeMarca
            };

            try
            {
                await _marcaRepository.AddAsync(marca);

                var output = new MarcaOutputDTO
                {
                    IdMarca = marca.IdMarca,
                    NomeMarca = marca.NomeMarca
                };

                return CreatedAtAction(nameof(Get), new { id = marca.IdMarca }, output);
            }
            catch (Exception)
            {
                return Conflict("Já existe uma marca com esse nome.");
            }
        }

        // PUT: api/Marca/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] MarcaInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var marcaExistente = await _marcaRepository.GetByIdAsync(id);
            if (marcaExistente == null)
                return NotFound();

            marcaExistente.NomeMarca = dto.NomeMarca;

            try
            {
                await _marcaRepository.UpdateAsync(marcaExistente);
                return NoContent();
            }
            catch (Exception)
            {
                return Conflict("Não foi possível atualizar a marca.");
            }
        }

        // DELETE: api/Marca/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id);
            if (marca == null)
                return NotFound();

            try
            {
                // mesma regra do MVC
                if (marca.Familias != null && marca.Familias.Count > 0)
                    return Conflict("A marca possui famílias associadas e não pode ser excluída.");

                await _marcaRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return Conflict("Não foi possível excluir a marca.");
            }
        }
    }
}

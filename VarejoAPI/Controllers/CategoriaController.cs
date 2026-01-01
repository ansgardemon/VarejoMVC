using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using VarejoAPI.DTO;
using Varejo.Models;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaController(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        // GET: api/Categoria
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaOutputDTO>>> Get()
        {
            var categorias = await _categoriaRepository.GetAllAsync();

            var resultado = categorias.Select(c => new CategoriaOutputDTO
            {
                IdCategoria = c.IdCategoria,
                DescricaoCategoria = c.DescricaoCategoria
            });

            return Ok(resultado);
        }

        // GET: api/Categoria/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoriaOutputDTO>> Get(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);

            if (categoria == null)
                return NotFound();

            var resultado = new CategoriaOutputDTO
            {
                IdCategoria = categoria.IdCategoria,
                DescricaoCategoria = categoria.DescricaoCategoria
            };

            return Ok(resultado);
        }

        // POST: api/Categoria
        [HttpPost]
        public async Task<ActionResult<CategoriaOutputDTO>> Post([FromBody] CategoriaInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoria = new Categoria
            {
                DescricaoCategoria = dto.DescricaoCategoria
            };

            try
            {
                await _categoriaRepository.AddAsync(categoria);

                var output = new CategoriaOutputDTO
                {
                    IdCategoria = categoria.IdCategoria,
                    DescricaoCategoria = categoria.DescricaoCategoria
                };

                return CreatedAtAction(nameof(Get), new { id = categoria.IdCategoria }, output);
            }
            catch (Exception)
            {
                return Conflict("Já existe uma categoria com essa descrição.");
            }
        }

        // PUT: api/Categoria/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] CategoriaInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoriaExistente = await _categoriaRepository.GetByIdAsync(id);
            if (categoriaExistente == null)
                return NotFound();

            categoriaExistente.DescricaoCategoria = dto.DescricaoCategoria;

            try
            {
                await _categoriaRepository.UpdateAsync(categoriaExistente);
                return NoContent();
            }
            catch (Exception)
            {
                return Conflict("Não foi possível atualizar a categoria.");
            }
        }

        // DELETE: api/Categoria/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                return NotFound();

            try
            {
                await _categoriaRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return Conflict("Não foi possível excluir a categoria. Ela pode estar em uso.");
            }
        }
    }
}

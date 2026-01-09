using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoAPI.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamiliaController : ControllerBase
    {
        private readonly IFamiliaRepository _familiaRepository;

        public FamiliaController(IFamiliaRepository familiaRepository)
        {
            _familiaRepository = familiaRepository;
        }

        // GET: api/Familia
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FamiliaOutputDTO>>> Get()
        {
            var familias = await _familiaRepository.GetAllAsync();

            var resultado = familias.Select(f => new FamiliaOutputDTO
            {
                IdFamilia = f.IdFamilia,
                NomeFamilia = f.NomeFamilia,
                Ativo = f.Ativo,
                CategoriaId = f.CategoriaId,
                MarcaId = f.MarcaId
            });

            return Ok(resultado);
        }

        // GET: api/Familia/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<FamiliaOutputDTO>> Get(int id)
        {
            var familia = await _familiaRepository.GetByIdAsync(id);
            if (familia == null)
                return NotFound();

            var resultado = new FamiliaOutputDTO
            {
                IdFamilia = familia.IdFamilia,
                NomeFamilia = familia.NomeFamilia,
                Ativo = familia.Ativo,
                CategoriaId = familia.CategoriaId,
                MarcaId = familia.MarcaId
            };

            return Ok(resultado);
        }

        // GET: api/Familia/PorCategoria/3
        [HttpGet("PorCategoria/{categoriaId:int}")]
        public async Task<ActionResult<IEnumerable<FamiliaOutputDTO>>> GetPorCategoria(int categoriaId)
        {
            var familias = await _familiaRepository.GetByFamiliaCategory(categoriaId);

            var resultado = familias.Select(f => new FamiliaOutputDTO
            {
                IdFamilia = f.IdFamilia,
                NomeFamilia = f.NomeFamilia,
                Ativo = f.Ativo,
                CategoriaId = f.CategoriaId,
                MarcaId = f.MarcaId
            });

            return Ok(resultado);
        }

        // POST: api/Familia
        [HttpPost]
        public async Task<ActionResult<FamiliaOutputDTO>> Post([FromBody] FamiliaInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var familia = new Familia
            {
                NomeFamilia = dto.NomeFamilia,
                Ativo = dto.Ativo,
                CategoriaId = dto.CategoriaId,
                MarcaId = dto.MarcaId
            };

            try
            {
                await _familiaRepository.AddAsync(familia);

                return CreatedAtAction(nameof(Get), new { id = familia.IdFamilia }, new FamiliaOutputDTO
                {
                    IdFamilia = familia.IdFamilia,
                    NomeFamilia = familia.NomeFamilia,
                    Ativo = familia.Ativo,
                    CategoriaId = familia.CategoriaId,
                    MarcaId = familia.MarcaId
                });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        // PUT: api/Familia/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] FamiliaInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var familia = await _familiaRepository.GetByIdAsync(id);
            if (familia == null)
                return NotFound();

            familia.NomeFamilia = dto.NomeFamilia;
            familia.Ativo = dto.Ativo;
            familia.CategoriaId = dto.CategoriaId;
            familia.MarcaId = dto.MarcaId;

            try
            {
                await _familiaRepository.UpdateAsync(familia);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        // DELETE: api/Familia/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var familia = await _familiaRepository.GetByIdAsync(id);
            if (familia == null)
                return NotFound();

            try
            {
                await _familiaRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict("Não foi possível excluir a família. Verifique se há produtos associados.");
            }
        }
    }
}

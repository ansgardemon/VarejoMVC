using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO;

namespace VarejoSHARED.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoMovimentoController : ControllerBase
    {
        private readonly ITipoMovimentoRepository _tipoMovimentoRepository;

        public TipoMovimentoController(ITipoMovimentoRepository tipoMovimentoRepository)
        {
            _tipoMovimentoRepository = tipoMovimentoRepository;
        }

        // GET api/TipoMovimento
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoMovimentoOutputDTO>>> Get()
        {
            var tipos = await _tipoMovimentoRepository.GetAllAsync();

            var resultado = tipos.Select(t => new TipoMovimentoOutputDTO
            {
                IdTipoMovimento = t.IdTipoMovimento,
                DescricaoTipoMovimento = t.DescricaoTipoMovimento,
                IdEspecieMovimento = t.EspecieMovimentoId,
                DescricaoEspecie = t.EspecieMovimento.DescricaoEspecieMovimento
            }).ToList();

            return Ok(resultado);
        }

        // GET api/TipoMovimento/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoMovimentoOutputDTO>> Get(int id)
        {
            var tipo = await _tipoMovimentoRepository.GetByIdAsync(id);
            if (tipo == null)
                return NotFound();

            var resultado = new TipoMovimentoOutputDTO
            {
                IdTipoMovimento = tipo.IdTipoMovimento,
                DescricaoTipoMovimento = tipo.DescricaoTipoMovimento,
                IdEspecieMovimento = tipo.EspecieMovimentoId,
                DescricaoEspecie = tipo.EspecieMovimento.DescricaoEspecieMovimento
            };

            return Ok(resultado);
        }

        // GET api/TipoMovimento/especies
        [HttpGet("especies")]
        public async Task<ActionResult<IEnumerable<EspecieMovimento>>> GetEspecies()
        {
            var especies = await _tipoMovimentoRepository.GetAllEspeciesAsync();
            return Ok(especies);
        }

        // POST api/TipoMovimento
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TipoMovimentoInputDTO dto)
        {
            var tipo = new TipoMovimento
            {
                DescricaoTipoMovimento = dto.DescricaoTipoMovimento,
                EspecieMovimentoId = dto.IdEspecieMovimento
            };

            try
            {
                await _tipoMovimentoRepository.AddAsync(tipo);
            }
            catch
            {
                return Conflict("Não foi possível adicionar o tipo de movimento.");
            }

            return Ok(new { tipo.IdTipoMovimento });
        }

        // PUT api/TipoMovimento/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] TipoMovimentoUpdateDTO dto)
        {
            var tipo = await _tipoMovimentoRepository.GetByIdAsync(id);
            if (tipo == null)
                return NotFound();

            // espécie não pode ser alterada
            tipo.DescricaoTipoMovimento = dto.DescricaoTipoMovimento;

            try
            {
                await _tipoMovimentoRepository.UpdateAsync(tipo);
            }
            catch
            {
                return Conflict("Não foi possível atualizar o tipo de movimento.");
            }

            return Ok();
        }
    }
}
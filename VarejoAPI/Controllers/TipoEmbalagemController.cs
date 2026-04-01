using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO;

namespace VarejoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoEmbalagemController : ControllerBase
    {
        private readonly ITipoEmbalagemRepository _tipoEmbalagemRepository;

        public TipoEmbalagemController(ITipoEmbalagemRepository tipoEmbalagemRepository)
        {
            _tipoEmbalagemRepository = tipoEmbalagemRepository;
        }

        // 🔹 GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoEmbalagemOutputDTO>>> GetAll()
        {
            var tiposEmbalagem = await _tipoEmbalagemRepository.GetAllAsync();

            var output = tiposEmbalagem.Select(t => new TipoEmbalagemOutputDTO
            {
                TipoEmbalagemId = t.IdTipoEmbalagem,
                DescricaoTipoEmbalagem = t.DescricaoTipoEmbalagem,
                Multiplicador = t.Multiplicador
                // Nota: ProdutoEmbalagemId precisaria ser mapeado de algum item da ICollection se necessário
            }).ToList();

            return Ok(output);
        }

        // 🔹 GET por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoEmbalagemOutputDTO>> Get(int id)
        {
            var tipoEmbalagem = await _tipoEmbalagemRepository.GetByIdAsync(id);

            if (tipoEmbalagem == null)
                return NotFound();

            var output = new TipoEmbalagemOutputDTO
            {
                TipoEmbalagemId = tipoEmbalagem.IdTipoEmbalagem,
                DescricaoTipoEmbalagem = tipoEmbalagem.DescricaoTipoEmbalagem,
                Multiplicador = tipoEmbalagem.Multiplicador
            };

            return Ok(output);
        }

        // 🔹 POST
        [HttpPost]
        public async Task<ActionResult<TipoEmbalagemOutputDTO>> Post([FromBody] TipoEmbalagemInputDTO dto)
        {
            if (dto == null) return BadRequest();

            var tipoEmbalagem = new TipoEmbalagem
            {
                DescricaoTipoEmbalagem = dto.DescricaoTipoEmbalagem,
                Multiplicador = dto.Multiplicador
                // Se houver lógica para ProdutoEmbalagemId na criação, adicione aqui
            };

            await _tipoEmbalagemRepository.AddAsync(tipoEmbalagem);

            var output = new TipoEmbalagemOutputDTO
            {
                TipoEmbalagemId = tipoEmbalagem.IdTipoEmbalagem,
                DescricaoTipoEmbalagem = tipoEmbalagem.DescricaoTipoEmbalagem,
                Multiplicador = tipoEmbalagem.Multiplicador
            };

            return Ok(output);
        }

        // 🔹 PUT
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] TipoEmbalagemInputDTO dto)
        {
            var tipoEmbalagem = await _tipoEmbalagemRepository.GetByIdAsync(id);

            if (tipoEmbalagem == null)
                return NotFound();

            tipoEmbalagem.DescricaoTipoEmbalagem = dto.DescricaoTipoEmbalagem;
            tipoEmbalagem.Multiplicador = dto.Multiplicador;

            await _tipoEmbalagemRepository.UpdateAsync(tipoEmbalagem);

            return NoContent();
        }

        // 🔹 DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var tipoEmbalagem = await _tipoEmbalagemRepository.GetByIdAsync(id);

            if (tipoEmbalagem == null)
                return NotFound();

            await _tipoEmbalagemRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoAPI.DTO;

namespace VarejoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoEmbalagemController : ControllerBase
    {
        private readonly ITipoEmbalagemRepository _tipoEmbalagemRepository;
        private readonly IProdutoEmbalagemRepository _produtoEmbalagemRepository;

        public TipoEmbalagemController(
            ITipoEmbalagemRepository tipoEmbalagemRepository,
            IProdutoEmbalagemRepository produtoEmbalagemRepository)
        {
            _tipoEmbalagemRepository = tipoEmbalagemRepository;
            _produtoEmbalagemRepository = produtoEmbalagemRepository;
        }

        // 🔹 GET por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoEmbalagem>> Get(int id)
        {
            var tipoEmbalagem = await _tipoEmbalagemRepository.GetByIdAsync(id);
            if (tipoEmbalagem == null)
                return NotFound();

            return Ok(tipoEmbalagem);
        }

        // 🔹 POST
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TipoEmbalagemInputDTO dto)
        {
            // Valida se o ProdutoEmbalagem vinculado realmente existe
            var produtoEmbalagem = await _produtoEmbalagemRepository.GetByIdAsync(dto.ProdutoEmbalagemId);
            if (produtoEmbalagem == null)
                return BadRequest("Produto Embalagem inválido.");

            var tipoEmbalagem = new TipoEmbalagem
            {
                DescricaoTipoEmbalagem = dto.DescricaoTipoEmbalagem,
                Multiplicador = dto.Multiplicador,
                ProdutosEmbalagem = dto.ProdutoEmbalagemId
            };

            await _tipoEmbalagemRepository.AddAsync(tipoEmbalagem);

            var output = new TipoEmbalagemOutputDTO
            {
                TipoEmbalagemId = tipoEmbalagem.IdTipoEmbalagem,
                DescricaoTipoEmbalagem = tipoEmbalagem.DescricaoTipoEmbalagem,
                Multiplicador = tipoEmbalagem.Multiplicador,
                ProdutoEmbalagemId = tipoEmbalagem.IdProdutoEmbalagem
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

            // Opcional: Se permitir atualizar a chave estrangeira, seria ideal validar se ela existe igual no POST
            tipoEmbalagem.ProdutoEmbalagemId = dto.ProdutoEmbalagemId;

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
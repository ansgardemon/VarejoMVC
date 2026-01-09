using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;

namespace VarejoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoEmbalagemController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoEmbalagemRepository _produtoEmbalagemRepository;

        public ProdutoEmbalagemController(
            IProdutoRepository produtoRepository,
            IProdutoEmbalagemRepository produtoEmbalagemRepository)
        {
            _produtoRepository = produtoRepository;
            _produtoEmbalagemRepository = produtoEmbalagemRepository;
        }

        // 🔹 GET por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoEmbalagem>> Get(int id)
        {
            var embalagem = await _produtoEmbalagemRepository.GetByIdAsync(id);
            if (embalagem == null)
                return NotFound();

            return Ok(embalagem);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProdutoEmbalagemInputDTO dto)
        {
            var produto = await _produtoRepository.GetByIdAsync(dto.ProdutoId);
            if (produto == null)
                return BadRequest("Produto inválido.");

            var embalagem = new ProdutoEmbalagem
            {
                ProdutoId = dto.ProdutoId,
                Preco = dto.Preco,
                Ean = dto.Ean,
                TipoEmbalagemId = dto.TipoEmbalagemId
            };

            await _produtoEmbalagemRepository.AddAsync(embalagem);

            var output = new ProdutoEmbalagemOutputDTO
            {
                IdProdutoEmbalagem = embalagem.IdProdutoEmbalagem,
                ProdutoId = embalagem.ProdutoId,
                TipoEmbalagemId = embalagem.TipoEmbalagemId,
                Preco = embalagem.Preco,
                Ean = embalagem.Ean
            };

            return Ok(output);
        }


        // 🔹 PUT
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProdutoEmbalagemInputDTO dto)
        {
            var embalagem = await _produtoEmbalagemRepository.GetByIdAsync(id);
            if (embalagem == null)
                return NotFound();

            embalagem.Preco = dto.Preco;
            embalagem.Ean = dto.Ean;
            embalagem.TipoEmbalagemId = dto.TipoEmbalagemId;

            await _produtoEmbalagemRepository.UpdateAsync(embalagem);
            return NoContent();
        }

        // 🔹 DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var embalagem = await _produtoEmbalagemRepository.GetByIdAsync(id);
            if (embalagem == null)
                return NotFound();

            await _produtoEmbalagemRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}

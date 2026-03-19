using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoAPI.DTO;

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

        // 🔹 GET: api/produtoembalagem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoEmbalagemOutputDTO>> GetById(int id)
        {
            var e = await _produtoEmbalagemRepository.GetByIdAsync(id);
            if (e == null)
                return NotFound();

            return Ok(new ProdutoEmbalagemOutputDTO
            {
                IdProdutoEmbalagem = e.IdProdutoEmbalagem,
                ProdutoId = e.ProdutoId,
                TipoEmbalagemId = e.TipoEmbalagemId,
                Preco = e.Preco,
                Ean = e.Ean
            });
        }

        // 🔹 GET: api/produtoembalagem/produto/1
        [HttpGet("produto/{produtoId}")]
        public async Task<ActionResult<IEnumerable<ProdutoEmbalagemOutputDTO>>> GetByProduto(int produtoId)
        {
            var lista = await _produtoEmbalagemRepository.GetByProdutoIdAsync(produtoId);

            var result = lista.Select(e => new ProdutoEmbalagemOutputDTO
            {
                IdProdutoEmbalagem = e.IdProdutoEmbalagem,
                ProdutoId = e.ProdutoId,
                TipoEmbalagemId = e.TipoEmbalagemId,
                Preco = e.Preco,
                Ean = e.Ean
            });

            return Ok(result);
        }

        // 🔹 POST
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProdutoEmbalagemInputDTO dto)
        {
            var produto = await _produtoRepository.GetByIdAsync(dto.ProdutoId);
            if (produto == null)
                return BadRequest("Produto inválido.");

            // 🔥 Validação de EAN duplicado
            var eanExiste = await _produtoEmbalagemRepository.EanExisteAsync(dto.Ean);
            if (eanExiste)
                return BadRequest("Já existe uma embalagem com esse EAN.");

            var embalagem = new ProdutoEmbalagem
            {
                ProdutoId = dto.ProdutoId,
                Preco = dto.Preco,
                Ean = dto.Ean,
                TipoEmbalagemId = dto.TipoEmbalagemId
            };

            await _produtoEmbalagemRepository.AddAsync(embalagem);

            return CreatedAtAction(nameof(GetById),
                new { id = embalagem.IdProdutoEmbalagem },
                new ProdutoEmbalagemOutputDTO
                {
                    IdProdutoEmbalagem = embalagem.IdProdutoEmbalagem,
                    ProdutoId = embalagem.ProdutoId,
                    TipoEmbalagemId = embalagem.TipoEmbalagemId,
                    Preco = embalagem.Preco,
                    Ean = embalagem.Ean
                });
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

            // 🔥 REGRA IMPORTANTE
            var possuiMovimento = await _produtoRepository
                .ProdutoEmbalagemPossuiMovimentoAsync(id);

            if (possuiMovimento)
                return BadRequest("Não é possível excluir. Embalagem possui movimentos.");

            await _produtoEmbalagemRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}
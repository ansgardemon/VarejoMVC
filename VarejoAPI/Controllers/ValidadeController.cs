using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO;

namespace VarejoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidadeController : ControllerBase
    {
        private readonly IValidadeRepository _validadeRepository;
        private readonly IProdutoRepository _produtoRepository;

        public ValidadeController(IValidadeRepository validadeRepository,
                                  IProdutoRepository produtoRepository)
        {
            _validadeRepository = validadeRepository;
            _produtoRepository = produtoRepository;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ValidadeOutputDTO>>> GetAll()
        {
            var validades = await _validadeRepository.GetAllAsync();
            return Ok(validades.Select(Map));
        }

        // =========================
        // GET POR ESTOQUE
        // =========================
        [HttpGet("estoque/{emEstoque}")]
        public async Task<ActionResult<IEnumerable<ValidadeOutputDTO>>> GetByEstoque(bool emEstoque)
        {
            var validades = await _validadeRepository.GetByEstoqueAsync(emEstoque);
            return Ok(validades.Select(Map));
        }

        // =========================
        // GET POR ID
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<ValidadeOutputDTO>> GetById(int id)
        {
            var validade = await _validadeRepository.GetByIdAsync(id);
            if (validade == null)
                return NotFound();

            return Ok(Map(validade));
        }

        // =========================
        // POST
        // =========================
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ValidadeInputDTO dto)
        {
            if (dto.ProdutoId <= 0)
                return BadRequest("ProdutoId inválido.");

            var produto = await _produtoRepository.GetByIdAsync(dto.ProdutoId);
            if (produto == null)
                return BadRequest("Produto não existe.");

            var validade = new Validade
            {
                DataValidade = dto.DataValidade,
                EmEstoque = dto.EmEstoque,
                ProdutoId = dto.ProdutoId
            };

            await _validadeRepository.AddAsync(validade);

            return CreatedAtAction(nameof(GetById),
                new { id = validade.IdValidade },
                new ValidadeOutputDTO
                {
                    IdValidade = validade.IdValidade,
                    DataValidade = validade.DataValidade.ToString("dd/MM/yyyy"),
                    EmEstoque = validade.EmEstoque,
                    ProdutoId = validade.ProdutoId,
                    ProdutoNome = produto.NomeProduto
                });
        }

        // =========================
        // PUT
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ValidadeInputDTO dto)
        {
            var validade = await _validadeRepository.GetByIdAsync(id);
            if (validade == null)
                return NotFound();

            // valida FK
            var produto = await _produtoRepository.GetByIdAsync(dto.ProdutoId);
            if (produto == null)
                return BadRequest("Produto não existe.");

            validade.DataValidade = dto.DataValidade;
            validade.EmEstoque = dto.EmEstoque;
            validade.ProdutoId = dto.ProdutoId;

            await _validadeRepository.UpdateAsync(validade);

            return NoContent();
        }

        // =========================
        // DELETE (ESGOTAR)
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var validade = await _validadeRepository.GetByIdAsync(id);
            if (validade == null)
                return NotFound();

            await _validadeRepository.EsgotarAsync(id);

            return NoContent();
        }

        // =========================
        // MAPPER
        // =========================
        private ValidadeOutputDTO Map(Validade v)
        {
            return new ValidadeOutputDTO
            {
                IdValidade = v.IdValidade,
                DataValidade = v.DataValidade.ToString("dd/MM/yyyy"),
                EmEstoque = v.EmEstoque,
                ProdutoId = v.ProdutoId,
                ProdutoNome = v.Produto.NomeProduto
            };
        }
    }
}
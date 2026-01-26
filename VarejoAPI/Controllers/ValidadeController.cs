using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoAPI.DTO;

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

        // GET: api/Validade
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ValidadeOutputDTO>>> GetAll()
        {
            var validades = await _validadeRepository.GetAllAsync();

            var result = validades.Select(v => new ValidadeOutputDTO
            {
                IdValidade = v.IdValidade,
                DataValidade = v.DataValidade,
                EmEstoque = v.EmEstoque,
                ProdutoId = v.ProdutoId,
                ProdutoNome = v.Produto.NomeProduto
            });

            return Ok(result);
        }


        // GET: api/Validade/estoque/true
        [HttpGet("estoque/{emEstoque}")]
        public async Task<ActionResult<IEnumerable<ValidadeOutputDTO>>> GetByEstoque(bool emEstoque)
        {
            var validades = await _validadeRepository.GetByEstoqueAsync(emEstoque);


            var result = validades.Select(v => new ValidadeOutputDTO
            {
                IdValidade = v.IdValidade,
                DataValidade = v.DataValidade,
                EmEstoque = v.EmEstoque,
                ProdutoId = v.ProdutoId,
                ProdutoNome = v.Produto.NomeProduto
            });


            return Ok(result);
        }

        // GET: api/Validade/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ValidadeOutputDTO>> GetById(int id)
        {
            var v = await _validadeRepository.GetByIdAsync(id);
            if (v == null)
                return NotFound();

            return Ok(new ValidadeOutputDTO
            {
                IdValidade = v.IdValidade,
                DataValidade = v.DataValidade,
                EmEstoque = v.EmEstoque,
                ProdutoId = v.ProdutoId,
                ProdutoNome = v.Produto.NomeProduto
            });
        }

        // POST: api/Validade
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ValidadeInputDTO dto)
        {
            if (dto.ProdutoId == 0)
                return BadRequest("ProdutoId é obrigatório.");

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
                    DataValidade = validade.DataValidade,
                    EmEstoque = validade.EmEstoque,
                    ProdutoId = validade.ProdutoId,
                    ProdutoNome = (await _produtoRepository.GetByIdAsync(validade.ProdutoId)).NomeProduto
                });
        }

        // PUT: api/Validade/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ValidadeInputDTO dto)
        {
            var validade = await _validadeRepository.GetByIdAsync(id);
            if (validade == null)
                return NotFound();

            validade.DataValidade = dto.DataValidade;
            validade.EmEstoque = dto.EmEstoque;
            validade.ProdutoId = dto.ProdutoId;

            await _validadeRepository.UpdateAsync(validade);

            return NoContent();
        }

        // DELETE: api/Validade/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var validade = await _validadeRepository.GetByIdAsync(id);
            if (validade == null)
                return NotFound();

            await _validadeRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
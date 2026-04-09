using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;
using VarejoSHARED.DTO;

namespace Varejo.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueRepository _estoqueRepo;
        private readonly VarejoDbContext _context;

        public EstoqueController(IEstoqueRepository estoqueRepo, VarejoDbContext context)
        {
            _estoqueRepo = estoqueRepo;
            _context = context;
        }

        // 🔹 GET: api/estoque
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] EstoqueFiltroViewModel filtro)
        {
            var result = await _estoqueRepo.ObterEstoqueGeralAsync(filtro);
            return Ok(result);
        }

        // 🔹 GET: api/estoque/{produtoId}/config
        [HttpGet("{produtoId}/config")]
        public async Task<IActionResult> GetConfig(int produtoId)
        {
            if (produtoId <= 0)
                return BadRequest("Id inválido.");

            var entity = await _context.EstoquesConfig
                .Include(e => e.Produto)
                .FirstOrDefaultAsync(e => e.ProdutoId == produtoId);

            if (entity == null)
                return NotFound();

            return Ok(EstoqueMapper(entity));
        }

        // 🔹 POST: api/estoque/config (UPSERT)
        [HttpPost("config")]
        public async Task<IActionResult> CreateConfig([FromBody] EstoqueConfigInput input)
        {
            if (input == null)
                return BadRequest("Dados inválidos.");

            var entity = await _context.EstoquesConfig
                .Include(e => e.Produto)
                .FirstOrDefaultAsync(e => e.ProdutoId == input.ProdutoId);

            if (entity == null)
            {
                entity = new EstoqueConfig
                {
                    ProdutoId = input.ProdutoId
                };

                _context.EstoquesConfig.Add(entity);
            }

            entity.ProdutoId = input.ProdutoId;
            entity.EstoqueMinimo = input.EstoqueMinimo;
            entity.EstoqueMaximo = input.EstoqueMaximo;

            await _context.SaveChangesAsync();

            await _context.Entry(entity)
                .Reference(e => e.Produto)
                .LoadAsync();

            return Ok(EstoqueMapper(entity));
        }

        // 🔹 PUT: api/estoque/config/{id}
        [HttpPut("config/{produtoId}")]
        public async Task<IActionResult> UpdateConfig(int produtoId, [FromBody] EstoqueConfigOutput input)
        {
            var entity = await _context.EstoquesConfig
                .Include(e => e.Produto)
                .FirstOrDefaultAsync(e => e.ProdutoId == produtoId);

            if (entity == null)
                return NotFound();

            entity.EstoqueMinimo = input.EstoqueMinimo;
            entity.EstoqueMaximo = input.EstoqueMaximo;

            await _context.SaveChangesAsync();

            return Ok(EstoqueMapper(entity));
        }

        // 🔹 MAPPER
        private EstoqueConfigInput EstoqueMapper(EstoqueConfig entity)
        {
            if (entity == null) return null;

            return new EstoqueConfigInput
            {
                ProdutoId = entity.ProdutoId,
                EstoqueMinimo = entity.EstoqueMinimo,
                EstoqueMaximo = entity.EstoqueMaximo
            };
        }
    }
}
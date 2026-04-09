using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Models;
using Varejo.Interfaces;
using VarejoSHARED.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SnapshotController : ControllerBase
    {
        private readonly VarejoDbContext _context;
        private readonly IEstoqueRepository _estoqueRepo;

        public SnapshotController(VarejoDbContext context, IEstoqueRepository estoqueRepo)
        {
            _context = context;
            _estoqueRepo = estoqueRepo;
        }

        // 🔹 GET: api/snapshot/produto/{produtoId}
        [HttpGet("produto/{produtoId}")]
        public async Task<IActionResult> GetByProduto(int produtoId)
        {
            var snapshots = await _context.EstoquesSnapshot
                .Where(s => s.ProdutoId == produtoId)
                .OrderByDescending(s => s.Data)
                .Select(s => new EstoqueSnapshotOutputDTO
                {
                    Id = s.Id,
                    ProdutoId = s.ProdutoId,
                    Data = s.Data,
                    Estoque = s.Estoque
                })
                .ToListAsync();

            if (!snapshots.Any())
                return NotFound("Nenhum snapshot encontrado para este produto.");

            return Ok(snapshots);
        }

        // 🔹 GET: api/snapshot/data/{data}
        [HttpGet("data/{data}")]
        public async Task<IActionResult> GetByData(string data)
        {
            if (!DateTime.TryParseExact(data, "dd-MM-yyyy", null,
                System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                return BadRequest("Formato de data inválido. Use dd-MM-yyyy.");
            }

            var snapshots = await _context.EstoquesSnapshot
                .Where(s => s.Data == parsedDate.Date)
                .Select(s => new EstoqueSnapshotOutputDTO
                {
                    Id = s.Id,
                    ProdutoId = s.ProdutoId,
                    Data = s.Data,
                    Estoque = s.Estoque
                })
                .ToListAsync();

            if (!snapshots.Any())
                return NotFound("Nenhum snapshot encontrado para esta data.");

            return Ok(snapshots);
        }

        // 🔹 POST: api/snapshot/gerar
        [HttpPost("gerar")]
        public async Task<IActionResult> GerarSnapshot()
        {
            var total = await _estoqueRepo.GerarSnapshotDiarioAsync();

            var result = new
            {
                Mensagem = "Snapshot diário gerado com sucesso.",
                QuantidadeRegistros = total,
                Data = DateTime.Now.Date
            };

            return Ok(result);
        }
    }
}

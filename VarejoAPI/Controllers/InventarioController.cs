using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioRepository _inventarioRepository;
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly VarejoDbContext _context;

        public InventarioController(
            IInventarioRepository inventarioRepository,
            IEstoqueRepository estoqueRepository,
            VarejoDbContext context)
        {
            _inventarioRepository = inventarioRepository;
            _estoqueRepository = estoqueRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventarioOutputDTO>>> Get()
        {
            var inventarios = await ObterInventariosCompletosQuery()
                .OrderByDescending(i => i.Id)
                .ToListAsync();

            return Ok(inventarios.Select(MapInventarioOutputDTO));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<InventarioOutputDTO>> Get(int id)
        {
            var inventario = await ObterInventarioCompletoAsync(id);

            if (inventario == null)
                return NotFound("Inventário não encontrado.");

            return Ok(MapInventarioOutputDTO(inventario));
        }

        [HttpGet("abertos")]
        public async Task<ActionResult<IEnumerable<InventarioOutputDTO>>> GetAbertos()
        {
            var inventarios = await ObterInventariosCompletosQuery()
                .Where(i => !i.Finalizado)
                .OrderByDescending(i => i.Id)
                .ToListAsync();

            return Ok(inventarios.Select(MapInventarioOutputDTO));
        }

        [HttpGet("finalizados")]
        public async Task<ActionResult<IEnumerable<InventarioOutputDTO>>> GetFinalizados()
        {
            var inventarios = await ObterInventariosCompletosQuery()
                .Where(i => i.Finalizado)
                .OrderByDescending(i => i.Id)
                .ToListAsync();

            return Ok(inventarios.Select(MapInventarioOutputDTO));
        }

        [HttpPost]
        public async Task<ActionResult<InventarioOutputDTO>> Post([FromBody] InventarioInputDTO dto)
        {
            var inventario = new Inventario
            {
                Data = dto.Data == default ? DateTime.Now : dto.Data,
                Observacao = dto.Observacao,
                Finalizado = false
            };

            await _inventarioRepository.CriarInventarioAsync(inventario);

            var inventarioCriado = await ObterInventarioCompletoAsync(inventario.Id);
            if (inventarioCriado == null)
                return StatusCode(500, "Não foi possível carregar o inventário criado.");

            return CreatedAtAction(
                nameof(Get),
                new { id = inventarioCriado.Id },
                MapInventarioOutputDTO(inventarioCriado));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] InventarioInputDTO dto)
        {
            var inventario = await _context.Inventarios.FirstOrDefaultAsync(i => i.Id == id);

            if (inventario == null)
                return NotFound("Inventário não encontrado.");

            if (inventario.Finalizado)
                return Conflict("Inventário finalizado não pode ser editado.");

            if (string.IsNullOrWhiteSpace(dto.Observacao))
                return BadRequest("A observação é obrigatória.");

            inventario.Data = dto.Data == default ? inventario.Data : dto.Data;
            inventario.Observacao = dto.Observacao;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id:int}/finalizar")]
        public async Task<ActionResult<InventarioOutputDTO>> Finalizar(int id)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Itens)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventario == null)
                return NotFound("Inventário não encontrado.");

            if (inventario.Finalizado)
                return Conflict("Inventário já está finalizado.");

            try
            {
                var novoMovimento = new Movimento
                {
                    Documento = inventario.Id,
                    Observacao = $"Ajuste via Inventário #{inventario.Id} - {inventario.Observacao ?? "Sem obs"}",
                    DataMovimento = DateTime.Now,
                    TipoMovimentoId = 7,
                    PessoaId = 1,
                    ProdutosMovimento = new List<ProdutoMovimento>()
                };

                _context.Movimentos.Add(novoMovimento);
                await _context.SaveChangesAsync();

                foreach (var item in inventario.Itens)
                {
                    var produtoMovimento = new ProdutoMovimento
                    {
                        ProdutoId = item.ProdutoId,
                        ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                        Quantidade = item.QuantidadeContada,
                        MovimentoId = novoMovimento.IdMovimento
                    };

                    _context.ProdutosMovimento.Add(produtoMovimento);
                }

                var totaisParaEstoque = inventario.Itens
                    .GroupBy(it => it.ProdutoId)
                    .Select(g => new
                    {
                        ProdutoId = g.Key,
                        SomaTotalUnidades = g.Sum(x => x.QuantidadeContada)
                    });

                foreach (var consolidado in totaisParaEstoque)
                {
                    await _estoqueRepository.AjustarEstoqueInventarioAsync(
                        consolidado.ProdutoId,
                        novoMovimento.IdMovimento,
                        inventario.Id,
                        consolidado.SomaTotalUnidades,
                        novoMovimento.Observacao);
                }

                inventario.Finalizado = true;
                _context.Inventarios.Update(inventario);

                await _context.SaveChangesAsync();

                var inventarioFinalizado = await ObterInventarioCompletoAsync(id);
                if (inventarioFinalizado == null)
                    return StatusCode(500, "Inventário finalizado, mas não foi possível recarregar os dados.");

                return Ok(MapInventarioOutputDTO(inventarioFinalizado));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Itens)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventario == null)
                return NotFound("Inventário não encontrado.");

            if (inventario.Finalizado)
                return Conflict("Não é possível excluir um inventário finalizado.");

            if (inventario.Itens != null && inventario.Itens.Any())
                return Conflict("O inventário possui itens lançados e não pode ser excluído.");

            try
            {
                await _inventarioRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private IQueryable<Inventario> ObterInventariosCompletosQuery()
        {
            return _context.Inventarios
                .Include(i => i.Itens)
                    .ThenInclude(it => it.Produto)
                .Include(i => i.Itens)
                    .ThenInclude(it => it.ProdutoEmbalagem)
                        .ThenInclude(pe => pe.TipoEmbalagem);
        }

        private async Task<Inventario?> ObterInventarioCompletoAsync(int id)
        {
            return await ObterInventariosCompletosQuery()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        private static InventarioOutputDTO MapInventarioOutputDTO(Inventario inventario)
        {
            return new InventarioOutputDTO
            {
                Id = inventario.Id,
                Data = inventario.Data,
                Observacao = inventario.Observacao,
                Finalizado = inventario.Finalizado,
                Itens = inventario.Itens?
                    .Select(MapInventarioItemOutputDTO)
                    .ToList() ?? new List<InventarioItemOutputDTO>()
            };
        }

        private static InventarioItemOutputDTO MapInventarioItemOutputDTO(InventarioItem item)
        {
            return new InventarioItemOutputDTO
            {
                Id = item.Id,
                InventarioId = item.InventarioId,
                ProdutoId = item.ProdutoId,
                NomeProduto = item.Produto?.NomeProduto,
                ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                DescricaoEmbalagem = item.ProdutoEmbalagem?.TipoEmbalagem?.DescricaoTipoEmbalagem,
                Multiplicador = item.ProdutoEmbalagem?.TipoEmbalagem?.Multiplicador ?? 1,
                QuantidadeSistema = item.QuantidadeSistema,
                QuantidadeContada = item.QuantidadeContada,
                Diferenca = item.Diferenca
            };
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using VarejoSHARED.DTO.Financeiro.PagamentoTitulo;

namespace VarejoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoTituloController : ControllerBase
    {
        private readonly IPagamentoTituloRepository _pagamentoRepo;

        public PagamentoTituloController(IPagamentoTituloRepository pagamentoRepo)
        {
            _pagamentoRepo = pagamentoRepo;
        }

        // GET api/PagamentoTitulo/titulo/{tituloId}
        [HttpGet("titulo/{tituloId}")]
        public async Task<ActionResult<IEnumerable<PagamentoTituloOutputDTO>>> GetByTitulo(int tituloId)
        {
            var pagamentos = await _pagamentoRepo.GetByTituloIdAsync(tituloId);

            var resultado = pagamentos.Select(p => new PagamentoTituloOutputDTO
            {
                IdPagamento = p.IdPagamento,
                ValorPago = p.ValorPago,
                DataPagamento = p.DataPagamento,
                Observacao = p.Observacao
            }).ToList();

            return Ok(resultado);
        }

        // POST api/PagamentoTitulo
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PagamentoTituloInputDTO dto)
        {
            try
            {
                await _pagamentoRepo.RegistrarPagamentoAsync(dto.TituloFinanceiroId, dto.ValorPago, dto.DataPagamento);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }

            return Ok(new
            {
                mensagem = "Pagamento registrado com sucesso."
            });
        }
    }
}
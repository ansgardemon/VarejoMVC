using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO.Financeiro.PrazoPagamento;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrazoPagamentoController : ControllerBase
    {

        private readonly IPrazoPagamentoRepository _prazoPagamentoRepository;

        public PrazoPagamentoController(IPrazoPagamentoRepository prazoPagamentoRepository)
        {
            _prazoPagamentoRepository = prazoPagamentoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<PrazoPagamentoOutputDTO>>> GetAll()
        {
            var lista = await _prazoPagamentoRepository.GetAllAsync();

            var result = lista.Select(p => new PrazoPagamentoOutputDTO
            {
                IdPrazoPagamento = p.IdPrazoPagamento,
                Descricao = p.Descricao,
                NumeroParcelas = p.NumeroParcelas,
                IntervaloDias = p.IntervaloDias
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrazoPagamentoOutputDTO>> GetById(int id)
        {
            var p = await _prazoPagamentoRepository.GetByIdAsync(id);

            if (p == null)
                return NotFound();

            var result = new PrazoPagamentoOutputDTO
            {
                IdPrazoPagamento = p.IdPrazoPagamento,
                Descricao = p.Descricao,
                NumeroParcelas = p.NumeroParcelas,
                IntervaloDias = p.IntervaloDias
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(PrazoPagamentoInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var prazo = new PrazoPagamento
            {
                Descricao = dto.Descricao,
                NumeroParcelas = dto.NumeroParcelas,
                IntervaloDias = dto.IntervaloDias
            };

            await _prazoPagamentoRepository.AddAsync(prazo);

            var result = new PrazoPagamentoOutputDTO
            {
                IdPrazoPagamento = prazo.IdPrazoPagamento,
                Descricao = prazo.Descricao,
                NumeroParcelas = prazo.NumeroParcelas,
                IntervaloDias = prazo.IntervaloDias
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, PrazoPagamentoInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var prazo = await _prazoPagamentoRepository.GetByIdAsync(id);

            if (prazo == null)
                return NotFound();

            prazo.Descricao = dto.Descricao;
            prazo.NumeroParcelas = dto.NumeroParcelas;
            prazo.IntervaloDias = dto.IntervaloDias;

            await _prazoPagamentoRepository.UpdateAsync(prazo);

            var result = new PrazoPagamentoOutputDTO
            {
                IdPrazoPagamento = prazo.IdPrazoPagamento,
                Descricao = prazo.Descricao,
                NumeroParcelas = prazo.NumeroParcelas,
                IntervaloDias = prazo.IntervaloDias
            };

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var prazo = await _prazoPagamentoRepository.GetByIdAsync(id);

            if (prazo == null)
                return NotFound();

            await _prazoPagamentoRepository.DeleteAsync(id);

            return Ok();

        }

    }
}

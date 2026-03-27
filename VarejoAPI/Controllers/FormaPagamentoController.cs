using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO.Financeiro.formaPagamento;
using VarejoSHARED.DTO.Financeiro.FormaPagamento;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormaPagamentoController : ControllerBase
    {
        private readonly IFormaPagamentoRepository _iFormaPagamentoRepository;

        public FormaPagamentoController(IFormaPagamentoRepository iFormaPagamentoRepository)
        {
            _iFormaPagamentoRepository = iFormaPagamentoRepository;
        }

        // GET 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FormaPagamentoOutputDTO>>> GetAll()
        {
            var lista = await _iFormaPagamentoRepository.GetAllAsync();

            var result = lista.Select(f => new FormaPagamentoOutputDTO
            {
                IdFormaPagamento = f.IdFormaPagamento,
                DescricaoFormaPagamento = f.DescricaoFormaPagamento
            });

            return Ok(result);
        }

        // GET 
        [HttpGet("{id}")]
        public async Task<ActionResult<FormaPagamentoOutputDTO>> GetById(int id)
        {
            var f = await _iFormaPagamentoRepository.GetByIdAsync(id);

            if (f == null)
                return NotFound();

            var dto = new FormaPagamentoOutputDTO
            {
                IdFormaPagamento = f.IdFormaPagamento,
                DescricaoFormaPagamento = f.DescricaoFormaPagamento
            };

            return Ok(dto);
        }

        // POST
        [HttpPost]
        public async Task<ActionResult> Create(FormaPagamentoInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pag = new FormaPagamento
            {
                DescricaoFormaPagamento = dto.DescricaoFormaPagamento
            };

            await _iFormaPagamentoRepository.AddAsync(pag);

            var result = new FormaPagamentoOutputDTO
            {
                IdFormaPagamento = pag.IdFormaPagamento,
                DescricaoFormaPagamento = pag.DescricaoFormaPagamento
            };

            return Ok(result);
        }

        // EDIT
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, FormaPagamentoInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var up = await _iFormaPagamentoRepository.GetByIdAsync(id);

            if (up == null)
                return NotFound();

            up.DescricaoFormaPagamento = dto.DescricaoFormaPagamento;

            await _iFormaPagamentoRepository.UpdateAsync(up);

            var result = new FormaPagamentoOutputDTO
            {
                IdFormaPagamento = up.IdFormaPagamento,
                DescricaoFormaPagamento = up.DescricaoFormaPagamento
            };

            return Ok(result);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var dt = await _iFormaPagamentoRepository.GetByIdAsync(id);

            if (dt == null)
                return NotFound();

            await _iFormaPagamentoRepository.DeleteAsync(id);

            return Ok();
        }
    }
}

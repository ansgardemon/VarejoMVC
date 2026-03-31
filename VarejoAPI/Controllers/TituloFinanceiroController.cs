using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoSHARED.DTO.Financeiro.PagamentoTitulo;
using VarejoSHARED.DTO.Financeiro.tituloFinanceiro;
using VarejoSHARED.DTO.Financeiro.TituloFinanceiro;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TituloFinanceiroController : ControllerBase
    {
        private readonly ITituloFinanceiroRepository _iTituloFinanceiroRepository;

        public TituloFinanceiroController(ITituloFinanceiroRepository iTituloFinanceiroRepository)
        {
            _iTituloFinanceiroRepository = iTituloFinanceiroRepository;
        }


        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var lista = await _iTituloFinanceiroRepository.GetAllAsync();

            var result = lista.Select(MapToDTO);

            return Ok(result);
        }


        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var t = await _iTituloFinanceiroRepository.GetByIdAsync(id);

            if (t == null)
                return NotFound();

            return Ok(MapToDTO(t));
        }


        [HttpPost]
        public async Task<ActionResult> Create(TituloFinanceiroInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var model = new TituloFinanceiro
            {
                Documento = dto.Documento,
                Parcela = dto.Parcela,
                Observacao = dto.Observacao,
                Valor = dto.Valor,
                DataEmissao = dto.DataEmissao,
                DataVencimento = dto.DataVencimento,
                EspecieTituloId = dto.EspecieTituloId,
                FormaPagamentoId = dto.FormaPagamentoId,
                PrazoPagamentoId = dto.PrazoPagamentoId,
                PessoaId = dto.PessoaId
            };

            model.AtualizarValores();

            await _iTituloFinanceiroRepository.AddAsync(model);

            return Ok(new TituloFinanceiroOutputDTO
            {
                IdTituloFinanceiro = model.IdTituloFinanceiro,
                Documento = model.Documento,
                Parcela = model.Parcela,
                Observacao = model.Observacao,
                Valor = model.Valor,
                ValorAberto = model.ValorAberto,
                DataEmissao = model.DataEmissao,
                DataVencimento = model.DataVencimento,
                Quitado = model.Quitado
            });
        }

    
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, TituloFinanceiroInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var model = await _iTituloFinanceiroRepository.GetByIdAsync(id);
            if (model == null)
                return NotFound();

            
            model.Documento = dto.Documento;
            model.Parcela = dto.Parcela;
            model.Observacao = dto.Observacao;
            model.Valor = dto.Valor;
            model.DataEmissao = dto.DataEmissao;
            model.DataVencimento = dto.DataVencimento;
            model.EspecieTituloId = dto.EspecieTituloId;
            model.FormaPagamentoId = dto.FormaPagamentoId;
            model.PrazoPagamentoId = dto.PrazoPagamentoId;
            model.PessoaId = dto.PessoaId;

            model.AtualizarValores();

            await _iTituloFinanceiroRepository.UpdateAsync(model);

            return await GetById(id);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _iTituloFinanceiroRepository.GetByIdAsync(id);

            if (model == null)
                return NotFound();

            if (model.Pagamentos?.Any() == true)
                return BadRequest("Não é possível excluir um título com pagamentos.");

            await _iTituloFinanceiroRepository.DeleteAsync(id);

            return Ok(new
            {
                message = "Título excluído com sucesso",
                id
            });

        }


        private TituloFinanceiroOutputDTO MapToDTO(TituloFinanceiro t)
        {
            return new TituloFinanceiroOutputDTO
            {
                IdTituloFinanceiro = t.IdTituloFinanceiro,
                Documento = t.Documento,
                Parcela = t.Parcela,
                Observacao = t.Observacao,
                Valor = t.Valor,
                ValorAberto = t.ValorAberto,
                DataEmissao = t.DataEmissao,
                DataVencimento = t.DataVencimento,
                DataPagamento = t.DataPagamento,
                Quitado = t.Quitado,

                EspecieTitulo = t.EspecieTitulo?.Descricao,
                FormaPagamento = t.FormaPagamento?.DescricaoFormaPagamento,
                PrazoPagamento = t.PrazoPagamento?.Descricao,
                Pessoa = t.Pessoa?.NomeRazao,

                Pagamentos = t.Pagamentos?.Select(p => new PagamentoTituloOutputDTO
                {
                    IdPagamento = p.IdPagamento,
                    ValorPago = p.ValorPago,
                    DataPagamento = p.DataPagamento,
                    Observacao = p.Observacao
                }).ToList()
            };
        }


    }
}
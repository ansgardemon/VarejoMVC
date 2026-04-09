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
            try
            {
                var lista = await _iTituloFinanceiroRepository.GetAllAsync();
                return Ok(lista.Select(MapToDTO));
            }
            catch
            {
                return StatusCode(500, "Erro ao buscar títulos.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Id inválido.");

            try
            {
                var t = await _iTituloFinanceiroRepository.GetByIdAsync(id);

                if (t == null)
                    return NotFound();

                return Ok(MapToDTO(t));
            }
            catch
            {
                return StatusCode(500, "Erro ao buscar título.");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(TituloFinanceiroInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Valor <= 0)
                return BadRequest("O valor do título deve ser maior que zero.");

            try
            {
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

                var criado = await _iTituloFinanceiroRepository.GetByIdAsync(model.IdTituloFinanceiro);

                return Ok(MapToDTO(criado));
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, TituloFinanceiroInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest("Id inválido.");

            try
            {
                var model = await _iTituloFinanceiroRepository.GetByIdAsync(id);

                if (model == null)
                    return NotFound();

                if (model.Pagamentos?.Any() == true)
                    return BadRequest("Não é possível alterar um título que já possui pagamentos.");

                if (dto.Valor <= 0)
                    return BadRequest("O valor do título deve ser maior que zero.");

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

                var atualizado = await _iTituloFinanceiroRepository.GetByIdAsync(id);

                return Ok(MapToDTO(atualizado));
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Id inválido.");

            try
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
            catch
            {
                return StatusCode(500, "Erro ao excluir título.");
            }
        }

        [HttpGet("filtro")]
        public async Task<ActionResult> GetFiltro(
            string? documento,
            string? pessoa,
            string? status,
            DateTime? dataInicio,
            DateTime? dataFim)
        {
            try
            {
                var titulos = await _iTituloFinanceiroRepository.GetAllAsync();
                var query = titulos.AsQueryable();

                if (!dataInicio.HasValue && !dataFim.HasValue)
                {
                    dataInicio = DateTime.Today;
                    dataFim = DateTime.Today.AddDays(30);
                }

                if (dataInicio.HasValue)
                    query = query.Where(t => t.DataVencimento >= dataInicio.Value);

                if (dataFim.HasValue)
                    query = query.Where(t => t.DataVencimento <= dataFim.Value);

                if (string.IsNullOrEmpty(status) || status == "aberto")
                    query = query.Where(t => !t.Quitado);
                else if (status == "quitado")
                    query = query.Where(t => t.Quitado);

                if (!string.IsNullOrEmpty(documento))
                    query = query.Where(t => t.Documento.ToString().Contains(documento));

                if (!string.IsNullOrEmpty(pessoa))
                    query = query.Where(t => t.Pessoa != null && t.Pessoa.NomeRazao.Contains(pessoa));

                return Ok(query.Select(MapToDTO));
            }
            catch
            {
                return StatusCode(500, "Erro ao filtrar títulos.");
            }
        }


        [HttpGet("{id}/resumo")]
        public async Task<ActionResult> GetResumo(int id)
        {
            try
            {
                var t = await _iTituloFinanceiroRepository.GetByIdAsync(id);

                if (t == null)
                    return NotFound();

                var totalPago = t.Pagamentos?.Sum(p => p.ValorPago) ?? 0;

                return Ok(new
                {
                    t.IdTituloFinanceiro,
                    t.Documento,
                    t.Valor,
                    TotalPago = totalPago,
                    t.ValorAberto,
                    t.Quitado
                });
            }
            catch
            {
                return StatusCode(500, "Erro ao gerar resumo.");
            }
        }


        [HttpGet("eventosCalendario")]
        public async Task<ActionResult> GetEventos()
        {
            try
            {
                var titulos = await _iTituloFinanceiroRepository.GetAllAsync();

                var eventos = titulos.Select(t => new
                {
                    id = t.IdTituloFinanceiro,
                    title = $"{t.Documento} - {t.Parcela} | {t.ValorAberto:C}",
                    start = t.DataVencimento.ToString("yyyy-MM-dd"),
                    color = t.Quitado ? "#28a745" : "#dc3545"
                });

                return Ok(eventos);
            }
            catch
            {
                return StatusCode(500, "Erro ao carregar eventos.");
            }
        }

        // MAPPER
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
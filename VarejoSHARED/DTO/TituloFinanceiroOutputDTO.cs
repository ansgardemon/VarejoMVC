using System.ComponentModel.DataAnnotations;
using VarejoSHARED.DTO.Financeiro.PagamentoTitulo;

namespace VarejoSHARED.DTO.Financeiro.tituloFinanceiro
{
    public class TituloFinanceiroOutputDTO
    {
        public int IdTituloFinanceiro { get; set; }

        public int Documento { get; set; }

        public int Parcela { get; set; }

        public string? Observacao { get; set; }

        public decimal Valor { get; set; }

        public decimal ValorAberto { get; set; }

        public DateTime DataEmissao { get; set; }

        public DateTime DataVencimento { get; set; }

        public DateTime? DataPagamento { get; set; }

        public bool Quitado { get; set; }

        public string EspecieTitulo { get; set; }

        public string? FormaPagamento { get; set; }

        public string? PrazoPagamento { get; set; }

        public string? Pessoa { get; set; }

        public List<PagamentoTituloOutputDTO> Pagamentos { get; set; }

    }
}
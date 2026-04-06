using System.ComponentModel.DataAnnotations;

namespace VarejoSHARED.DTO.Financeiro.TituloFinanceiro
{
    public class TituloFinanceiroInputDTO
    {
        public int Documento { get; set; }
        public int Parcela { get; set; }
        public string? Observacao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }

        public int EspecieTituloId { get; set; }
        public int? FormaPagamentoId { get; set; }
        public int? PrazoPagamentoId { get; set; }
        public int? PessoaId { get; set; }
    }
}

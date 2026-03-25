namespace Varejo.ViewModels
{
    public class TituloFinanceiroViewModel
    {
        public int IdTituloFinanceiro { get; set; }

        public int Documento { get; set; }
        public int Parcela { get; set; }

        public string Observacao { get; set; }

        public decimal Valor { get; set; }
        public decimal? ValorPago { get; set; }
        public decimal ValorAberto { get; set; }

        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataPagamento { get; set; }

        public bool Quitado { get; set; }

        public int EspecieTituloId { get; set; }
        public string? EspecieDescricao { get; set; }

        public int? FormaPagamentoId { get; set; }
        public string? FormaDescricao { get; set; }

        public int? PrazoPagamentoId { get; set; }
        public string? PrazoDescricao { get; set; }

        public int? PessoaId { get; set; }
        public string? NomePessoa { get; set; }
    }
}
namespace Varejo.ViewModels
{
    public class PrazoPagamentoViewModel
    {
        public int IdPrazoPagamento { get; set; }

        public string Descricao { get; set; }

        public int NumeroParcelas { get; set; }

        public int IntervaloDias { get; set; }
    }
}
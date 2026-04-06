using System.ComponentModel.DataAnnotations;

namespace VarejoSHARED.DTO.Financeiro.PrazoPagamento
{
    public class PrazoPagamentoOutputDTO
    {
        public int IdPrazoPagamento { get; set; }

        public string Descricao { get; set; }

        public int NumeroParcelas { get; set; }

        public int IntervaloDias { get; set; }

    }
}
using System.ComponentModel.DataAnnotations;

namespace VarejoSHARED.DTO.Financeiro.PrazoPagamento
{
    public class PrazoPagamentoInputDTO
    {
        public string Descricao { get; set; }

        public int NumeroParcelas { get; set; }

        public int IntervaloDias { get; set; }
    }
}

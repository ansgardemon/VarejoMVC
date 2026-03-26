using System.ComponentModel.DataAnnotations;

namespace VarejoSHARED.DTO.Financeiro.PagamentoTitulo
{
    public class PagamentoTituloInputDTO
    {
        public int TituloFinanceiroId { get; set; }

        public decimal ValorPago { get; set; }

        public DateTime DataPagamento { get; set; }

        public string? Observacao { get; set; }
    }
}

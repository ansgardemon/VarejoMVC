using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class PagamentoTitulo
    {
        [Key]
        public int IdPagamento { get; set; }

        [Required]
        public int TituloFinanceiroId { get; set; }
        public TituloFinanceiro TituloFinanceiro { get; set; }

        [Required]
        public decimal ValorPago { get; set; }

        [Required]
        public DateTime DataPagamento { get; set; }

        [StringLength(100)]
        public string? Observacao { get; set; }
    }
}
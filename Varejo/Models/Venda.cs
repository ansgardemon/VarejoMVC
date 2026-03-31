using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varejo.Models
{
    public class Venda
    {
        [Key]
        public int IdVenda { get; set; }

        [Required]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Subtotal Itens")]
        public decimal ValorSubtotal { get; set; } // Soma dos subtotais dos itens

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Desconto Total")]
        public decimal DescontoTotal { get; set; } // Desconto dado no fechamento da venda

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Valor Final")]
        public decimal ValorFinal => ValorSubtotal - DescontoTotal;

        [StringLength(200)]
        public string? Observacao { get; set; }

        public bool Finalizada { get; set; } = false;

        // RELACIONAMENTOS
        [Required]
        public int PessoaId { get; set; }
        public virtual Pessoa? Pessoa { get; set; }

        [Required]
        public int FormaPagamentoId { get; set; }
        public virtual FormaPagamento? FormaPagamento { get; set; }

        public int? PrazoPagamentoId { get; set; }
        public virtual PrazoPagamento? PrazoPagamento { get; set; }

        public virtual ICollection<VendaItem> Itens { get; set; } = new List<VendaItem>();
    }
}
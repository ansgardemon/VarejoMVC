using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class FormaPagamento
    {
        [Key]
        public int IdFormaPagamento { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Forma Pagamento")]
        public string DescricaoFormaPagamento { get; set; }

    }
}

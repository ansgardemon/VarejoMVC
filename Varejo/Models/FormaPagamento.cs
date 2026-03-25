using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class FormaPagamento
    {
        [Key]
        public int IdFormaPagamento { get; set; }

        [Required]
        public string DescricaoFormaPagamento { get; set; }



        // RELACIONAMENTOS

        public ICollection<TituloFinanceiro> Titulos { get; set; }
    }
}

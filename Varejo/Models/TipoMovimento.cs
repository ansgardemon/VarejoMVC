using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class TipoMovimento
    {

        [Key]
        public int IdTipoMovimento { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Descrição Tipo Movimento")]
        public string DescricaoTipoMovimento { get; set; }


        // Relacionamento com EspecieMovimento
        [Display(Name = "Espécie Movimento")]
        public int EspecieMovimentoId { get; set; }
        public EspecieMovimento EspecieMovimento { get; set; }

    }
}

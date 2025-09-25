using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class TipoMovimento
    {

        [Key]
        public int IdTipoMovimento { get; set; }

        [Required]
        [StringLength(20)]
        public string DescricaoTipoMovimento { get; set; }


        // Relacionamento com EspecieMovimento
        public int EspecieMovimentoId { get; set; }
        public EspecieMovimento EspecieMovimento { get; set; }

    }
}

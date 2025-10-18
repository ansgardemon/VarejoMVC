using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class EspecieMovimento
    {
        [Key]
        public int IdEspecieMovimento { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Espécie Movimento")]
        public string DescricaoEspecieMovimento { get; set; }



        //RELACIONAMENTO UM PARA MUITOS
        [Display(Name = "Tipos de Movimento")]
        public ICollection<TipoMovimento> TiposMovimento { get; set; }


    }
}

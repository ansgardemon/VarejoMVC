using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class EspecieMovimento
    {
        [Key]
        public int IdEspecieMovimento { get; set; }

        [Required]
        [StringLength(10)]
        public string DescricaoEspecieMovimento { get; set; }



        //RELACIONAMENTO UM PARA MUITOS

        public ICollection<TipoMovimento> TiposMovimento { get; set; }


    }
}

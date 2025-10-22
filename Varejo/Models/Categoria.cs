using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Categoria
    {

        [Key]
        public int IdCategoria { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "Categoria")]
        public string DescricaoCategoria { get; set; }


        //RELACIONAMENTO UM PARA MUITOS
        [Display(Name = "Famílias")]
        public virtual ICollection<Familia>? Familias { get; set; }

    }
}

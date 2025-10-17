using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Marca
    {
        [Key]
        public int IdMarca { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Marca")]
        public string NomeMarca { get; set; }


        //RELACIONAMENTO UM PARA MUITOS
        [Display(Name = "Famílias")]
        public virtual ICollection<Familia>? Familias { get; set; }


    }
}

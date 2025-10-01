using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Categoria
    {
        private ICollection<Familia>? familias;

        [Key]
        public int IdCategoria { get; set; }


        [Required]
        [StringLength(50)]
        public string DescricaoCategoria { get; set; }


        //RELACIONAMENTO UM PARA MUITOS

        public virtual ICollection<Familia>? Familias { get; set; }

    }
}

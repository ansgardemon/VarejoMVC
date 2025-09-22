using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class TipoUsuario
    {

        [Key]
        public int IdTipoUsuario { get; set; }



        [Required]
        [StringLength(20)]
        public string DescricaoTipoUsuario { get; set; }

        
        
        public virtual ICollection<Usuario>? Usuarios { get; set; }



    }
}

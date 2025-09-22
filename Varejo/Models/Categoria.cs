using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Varejo.Models
{
    public class Categoria
    {

        [Key]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!")]
        [StringLength(50)]
        public string Nome { get; set; }

    }
}

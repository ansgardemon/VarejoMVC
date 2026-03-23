using System.ComponentModel.DataAnnotations;

namespace VarejoSHARED.DTO
{
    public class CategoriaInputDTO
    {
        [Required]
        [StringLength(100)]
        public string DescricaoCategoria { get; set; }
    }
}

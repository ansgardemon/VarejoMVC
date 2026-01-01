using System.ComponentModel.DataAnnotations;

namespace VarejoAPI.DTO
{
    public class CategoriaInputDTO
    {
        [Required]
        [StringLength(100)]
        public string DescricaoCategoria { get; set; }
    }
}

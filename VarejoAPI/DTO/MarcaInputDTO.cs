using System.ComponentModel.DataAnnotations;

namespace VarejoAPI.DTO
{
    public class MarcaInputDTO
    {
        [Required]
        [StringLength(100)]
        public string NomeMarca { get; set; }
    }
}

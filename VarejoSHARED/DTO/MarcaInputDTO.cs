using System.ComponentModel.DataAnnotations;

namespace VarejoSHARED.DTO
{
    public class MarcaInputDTO
    {
        [Required]
        [StringLength(100)]
        public string NomeMarca { get; set; }
    }
}

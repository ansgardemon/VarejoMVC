using System.ComponentModel.DataAnnotations;

namespace VarejoAPI.DTO
{
    public class FamiliaInputDTO
    {
        [Required]
        public string NomeFamilia { get; set; }

        public bool Ativo { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        public int? MarcaId { get; set; }
    }
}

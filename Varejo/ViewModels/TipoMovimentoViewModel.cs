using System.ComponentModel.DataAnnotations;

namespace Varejo.ViewModels
{
    public class TipoMovimentoViewModel
    {
        [Key]
        public int IdTipoMovimento { get; set; }

        [Required(ErrorMessage = "O nome do tipo de movimento é obrigatório.")]
        [StringLength(20, ErrorMessage = "O nome não pode ter mais de 20 caracteres.")]
        [Display(Name = "Tipo de Movimento")]
        public string DescricaoTipoMovimento { get; set; }

        // Apenas para exibição, sem validação
        public string? DescricaoEspecie { get; set; }

        [Required(ErrorMessage = "A espécie de movimento é obrigatória.")]
        [Display(Name = "Espécie de Movimento")]
        public int IdEspecieMovimento { get; set; }
    }
}

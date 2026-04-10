using System.ComponentModel.DataAnnotations;

namespace Varejo.ViewModels
{
    public class ParametroViewModel
    {
        [Required]
        public int TipoMovimentoVendaId { get; set; }

        [Required]
        public int TipoMovimentoCompraId { get; set; }

        [Required]
        public int TipoMovimentoAvariaId { get; set; }
        [Required]
        public int TipoMovimentoEntradaBonificacaoId { get; set; }
    }
}
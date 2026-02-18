using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class PedidoStatus
    {
        [Key]
        public int IdPedidoStatus { get; set; }

        [Required]
        public string DescricaoStatus { get; set; }
    }
}

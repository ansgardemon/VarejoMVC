using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varejo.Models
{
    public class ProdutoCusto
    {
        [Key]
        public int IdProdutoCusto { get; set; }

        [Required]
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }

        // Referência de onde veio esse custo (Vinculado ao Recebimento)
        public int RecebimentoId { get; set; }
        public virtual Recebimento Recebimento { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ValorCustoUnitario { get; set; }

        public DateTime DataRegistro { get; set; } = DateTime.Now;

        // Flag para saber qual é o custo ATUAL (o último que entrou)
        public bool EhCustoAtual { get; set; } = true;
    }
}

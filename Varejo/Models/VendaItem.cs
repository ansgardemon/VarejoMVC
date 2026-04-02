using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varejo.Models
{
    public class VendaItem
    {
        [Key]
        public int IdVendaItem { get; set; }

        [Required]
        public int VendaId { get; set; }
        public virtual Venda? Venda { get; set; }

        [Required]
        public int ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }

        [Required]
        public int ProdutoEmbalagemId { get; set; }
        public virtual ProdutoEmbalagem? ProdutoEmbalagem { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço Tabela")]
        public decimal ValorUnitario { get; set; } // Preço da embalagem no momento

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Desconto Unitário")]
        public decimal DescontoUnitario { get; set; } // Ex: R$ 0,50 de desconto por unidade

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Subtotal")]
        public decimal Subtotal => Quantidade * (ValorUnitario - DescontoUnitario);
    }
}
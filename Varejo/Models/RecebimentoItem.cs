using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Varejo.Models;

public class RecebimentoItem
{
    [Key]
    public int IdRecebimentoItem { get; set; }

    public int RecebimentoId { get; set; }
    [ForeignKey("RecebimentoId")]
    public virtual Recebimento Recebimento { get; set; }

    public int ProdutoId { get; set; }
    public virtual Produto Produto { get; set; }

    // No lugar do FatorConversao, usamos a relação forte:
    [Required]
    public int ProdutoEmbalagemId { get; set; }
    [ForeignKey("ProdutoEmbalagemId")]
    public virtual ProdutoEmbalagem ProdutoEmbalagem { get; set; }

    public string CodigoProdutoFornecedor { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Quantidade { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal ValorUnitario { get; set; }

    public bool EhBonificacao { get; set; } = false;

    // O Subtotal continua igual
    [NotMapped]
    public decimal Subtotal => Quantidade * ValorUnitario;
}
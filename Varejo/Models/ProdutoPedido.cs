using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class ProdutoPedido
    {
        [Key]
        public int IdProdutoPedido { get; set; }

        [Required]
        [Display(Name = "Quantidade")]
        public decimal Quantidade { get; set; }


        [Required]
        [Display(Name = "Valor Unitário")]
        public decimal ValorUnitario { get; set; }


        [Display(Name = "Desconto")]
        public decimal Desconto { get; set; }

        [Required]
        [Display(Name = "Valor Final")]
        public decimal ValorFinal { get; set; }


        //RELACIONAMENTO COM OUTRAS TABELAS

        [Display(Name = "Produto")]
        public int ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }

        [Display(Name = "Embalagem")]
        public int ProdutoEmbalagemId { get; set; }
        public virtual ProdutoEmbalagem? ProdutoEmbalagem { get; set; }

        [Display(Name = "Movimento")]
        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }


    }
}

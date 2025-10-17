using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class ProdutoMovimento
    {
        [Key]
        public int IdProdutoMovimento { get; set; }

        [Required]
        [Display(Name = "Quantidade")]
        public decimal Quantidade { get; set; }

        //RELACIONAMENTO COM OUTRAS TABELAS

        [Display(Name = "Produto")]
        public int ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }

        [Display(Name = "Embalagem")]
        public int ProdutoEmbalagemId { get; set; }
        public virtual ProdutoEmbalagem? ProdutoEmbalagem { get; set; }

        [Display(Name = "Movimento")]
        public int MovimentoId { get; set; }
        public virtual Movimento? Movimento { get; set; }






    }
}

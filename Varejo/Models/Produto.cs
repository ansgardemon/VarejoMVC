using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varejo.Models
{
    public class Produto
    {


        [Key]
        public int IdProduto { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Complemento")]
        public string Complemento { get; set; }


        [Required]
        [StringLength(101)]
        [Display(Name = "Produto")]
        public string NomeProduto { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Estoque Inicial")]
        public decimal EstoqueInicial { get; set; } = 0;

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;


        [StringLength(255)]
        [Required]
        [Display(Name = "URL Imagem")]
        public string UrlImagem { get; set; }


        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Custo Médio")]
        public decimal CustoMedio { get; set; }


        //RELACIONAMENTO COM OUTRA TABELA

        [Display(Name = "Família")]
        public int FamiliaId { get; set; }
        public virtual Familia? Familia { get; set; }

        //RELACIONAMENTO UM PARA MUITOS
        [Display(Name = "Embalagens")]
        public virtual ICollection<ProdutoEmbalagem>? ProdutosEmbalagem  { get; set; }
    }
}

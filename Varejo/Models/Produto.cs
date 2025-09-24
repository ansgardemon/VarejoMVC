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
        public string Complemento { get; set; }


        [Required]
        [StringLength(101)]
        public string NomeProduto { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal EstoqueInicial { get; set; } = 0;

        public bool Ativo { get; set; } = true;


        [StringLength(255)]
        [Required]
        public string UrlImagem { get; set; }


        [Column(TypeName = "decimal(18,4)")]
        public decimal CustoMedio { get; set; }


        //RELACIONAMENTO UM PARA MUITOS

        public int FamiliaId { get; set; }
        public virtual Familia? Familia { get; set; }


    }
}

using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;

namespace Varejo.Models
{
    public class ProdutoEmbalagem
    {

        [Key]
        public int IdProdutoEmbalagem { get; set; }


        [Required(ErrorMessage = "O preço do produto é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço do produto deve ser maior que zero.")]
        [Precision(18, 2)] // Define precisão decimal no banco (decimal(18,2))
        public decimal Preco { get; set; }

        [Required]
        public string Ean { get; set; }

        public bool Principal { get; set; }


        //RELACIONAMENTO COM OUTRAS TABELAS

        public int ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }

        public int TipoEmbalagemId { get; set; }
        public virtual TipoEmbalagem? TipoEmbalagem { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Validade
    {
        [Key]
        public int IdValidade { get; set; }

        [Required]
        public DateTime DataValidade { get; set; }

        public bool EmEstoque { get; set; } = true;


        //RELACIONAMENTO COM OUTRA TABELAS

        [Required]
        public int ProdutoId { get; set; }


        public virtual Produto? Produto { get; set; }




    }
}

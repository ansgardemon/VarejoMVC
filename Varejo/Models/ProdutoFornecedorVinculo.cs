using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class ProdutoFornecedorVinculo
    {
        [Key]
        public int Id { get; set; }

        // O nosso produto (Cadastro Interno)
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }

        // O fornecedor (Pessoa com EhFornecedor = true)
        public int PessoaId { get; set; }
        public virtual Pessoa Fornecedor { get; set; }

        [Required, StringLength(50)]
        public string CodigoProdutoNoFornecedor { get; set; } // O <cProd> do XML

        [StringLength(100)]
        public string DescricaoNoFornecedor { get; set; } // O <xProd> do XML para referência
    }
}

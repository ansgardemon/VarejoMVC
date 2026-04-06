namespace Varejo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace Varejo.Models
    {
        public class RecebimentoItem
        {
            [Key]
            public int IdRecebimentoItem { get; set; }

            // Relacionamento com o Cabeçalho da Nota
            [Required]
            public int RecebimentoId { get; set; }
            [ForeignKey("RecebimentoId")]
            public virtual Recebimento Recebimento { get; set; }

            // Relacionamento com o seu Cadastro de Produto atual
            [Required]
            [Display(Name = "Produto Interno")]
            public int ProdutoId { get; set; }
            public virtual Produto Produto { get; set; }

            [StringLength(50)]
            [Display(Name = "Código no Fornecedor")]
            public string CodigoProdutoFornecedor { get; set; } // O <cProd> do XML para o De-Para

            [Required]
            [Column(TypeName = "decimal(18,4)")]
            public decimal Quantidade { get; set; }

            [Required]
            [Column(TypeName = "decimal(18,4)")]
            [Display(Name = "Preço de Custo na Nota")]
            public decimal ValorUnitario { get; set; }

            [Display(Name = "Bonificação")]
            public bool EhBonificacao { get; set; } = false;

            // Fator para converter ex: Caixa para Unidade
            [Display(Name = "Fator Conversão")]
            public decimal FatorConversao { get; set; } = 1;

            // Propriedade calculada para facilitar exibições em listas
            [NotMapped]
            public decimal Subtotal => Quantidade * ValorUnitario;

            // Indica se este item gerou um novo registro na tabela ProdutoCusto
            public bool GerouCusto { get; set; } = true;
        }
    }
}

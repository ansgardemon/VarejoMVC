using System.ComponentModel.DataAnnotations;
using Varejo.Models;


namespace Varejo.Models {


    public class HistoricoProduto
    {
        [Key]
        public int Id { get; set; }

        public int ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }

        public int MovimentoId { get; set; }
        public virtual Movimento? Movimento { get; set; }

        public DateTime Data { get; set; }

  
        public int TipoMovimentoId { get; set; }
        public virtual TipoMovimento? TipoMovimento { get; set; }

 
        public int EspecieMovimentoId { get; set; }
        public virtual EspecieMovimento? EspecieMovimento { get; set; }

        public decimal QuantidadeMovimento { get; set; }
        public decimal EstoqueAntes { get; set; }
        public decimal EstoqueDepois { get; set; }
        public string? Observacao { get; set; }
    }
}
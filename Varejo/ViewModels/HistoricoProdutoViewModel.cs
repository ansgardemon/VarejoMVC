using Varejo.Models;

namespace Varejo.ViewModels
{
    public class HistoricoProdutoViewModel
    {
        public int Id { get; set; }

        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }

        public int MovimentoId { get; set; }

        public DateTime Data { get; set; }

        public int EspecieMovimentoId { get; set; }

        public decimal QuantidadeMovimento { get; set; }

        public decimal EstoqueAntes { get; set; }
        public decimal EstoqueDepois { get; set; }

        public string? Observacao { get; set; }


    }
}

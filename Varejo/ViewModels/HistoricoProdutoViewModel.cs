using Varejo.Models;

namespace Varejo.ViewModels
{
    public class HistoricoProdutoViewModel
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string TipoMovimento { get; set; }    // Ex: Venda, Compra
        public string EspecieMovimento { get; set; } // Ex: Entrada, Saída
        public decimal EstoqueAntes { get; set; }
        public decimal QuantidadeMovimento { get; set; }
        public decimal EstoqueDepois { get; set; }
        public string? Observacao { get; set; }
    }
}

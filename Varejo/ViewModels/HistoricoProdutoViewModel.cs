using Varejo.Models;

namespace Varejo.ViewModels
{
    public class HistoricoProdutoViewModel
    {
        public DateTime Data { get; set; }
        public string TipoMovimento { get; set; } // Aqui guardaremos a descrição
        public decimal EstoqueAntes { get; set; }
        public decimal QuantidadeMovimento { get; set; }
        public decimal EstoqueDepois { get; set; }
        public string? Observacao { get; set; }
        public int Id { get; set; }
    }
}

namespace Varejo.ViewModels
{
    public class EstoqueConfigViewModel
    {
        public int Id { get; set; } // 0 para novo, > 0 para editar
        public int ProdutoId { get; set; }
        public string? NomeProduto { get; set; } // Apenas para exibição

        public decimal EstoqueMinimo { get; set; }
        public decimal EstoqueMaximo { get; set; }
    }
}
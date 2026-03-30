namespace Varejo.ViewModels
{
    public class AddItemRequest
    {
        public int InventarioId { get; set; }
        public int ProdutoId { get; set; }
        public int ProdutoEmbalagemId { get; set; }
        public string QuantidadeContada { get; set; } // Recebemos como string para não dar pau de vírgula/ponto
    }
}

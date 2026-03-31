namespace Varejo.Models
{
    public class EstoqueSnapshot
    {
        public int Id { get; set; }

        public int ProdutoId { get; set; }

        public DateTime Data { get; set; }

        public decimal Estoque { get; set; }
    }
}

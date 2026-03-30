namespace Varejo.Models
{
    public class InventarioItem
    {
        public int Id { get; set; }

        public int InventarioId { get; set; }
        public Inventario Inventario { get; set; }

        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }

        // 🔥 NOVO
        public int ProdutoEmbalagemId { get; set; }
        public ProdutoEmbalagem ProdutoEmbalagem { get; set; }



        public decimal QuantidadeSistema { get; set; }
        public decimal QuantidadeContada { get; set; }

        public decimal Diferenca => QuantidadeContada - QuantidadeSistema;
    }
}

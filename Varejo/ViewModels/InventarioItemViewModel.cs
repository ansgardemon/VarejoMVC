namespace Varejo.ViewModels
{
    public class InventarioItemViewModel
    {
        public int Id { get; set; }

        public int ProdutoId { get; set; }
        public int ProdutoEmbalagemId { get; set; }


        public string DescricaoEmbalagem { get; set; }
        public int Multiplicador { get; set; }

        public string NomeProduto { get; set; }

        public decimal QuantidadeSistema { get; set; }
        public decimal QuantidadeContada { get; set; }

        public decimal Diferenca => QuantidadeContada - QuantidadeSistema;
    }
}

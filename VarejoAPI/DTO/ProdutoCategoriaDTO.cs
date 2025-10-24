namespace VarejoAPI.DTO
{
    public class ProdutoCategoriaDTO
    {

        public int IdCategoria { get; set; }

        public string DescricaoCategoria { get; set; }

        public int IdProduto { get; set; }

        public string NomeProduto { get; set; }


        public string UrlImagem { get; set; }

        public decimal Preco { get; set; }

    }
}

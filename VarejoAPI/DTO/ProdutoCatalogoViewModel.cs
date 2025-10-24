namespace VarejoAPI.DTO
{
    public class ProdutoCatalogoViewModel
    {
        public int IdProduto { get; set; }

        public int? IdCategoria { get; set; }

        public int? IdMarca { get; set; }
        public string NomeProduto { get; set; }
        public string? UrlImagem { get; set; }
        public decimal Preco { get; set; }

    }
}

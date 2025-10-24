namespace VarejoAPI.DTO
{
    public class ProdutoDTO
    {


        public int IdFamilia { get; set; }

        public string NomeFamilia { get; set; }

        public int IdProduto { get; set; }

        public string? Complemento { get; set; }

        public string NomeProduto { get; set; }

        public string DescricaoCategoria { get; set; }

        public string NomeMarca { get; set; }


        public decimal EstoqueInicial { get; set; }

        public decimal EstoqueAtual { get; set; }

        public bool Ativo { get; set; }

        public string UrlImagem { get; set; }

        public decimal CustoMedio { get; set; }

    }
}

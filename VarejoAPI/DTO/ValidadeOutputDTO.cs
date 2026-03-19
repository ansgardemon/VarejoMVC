namespace VarejoAPI.DTO
{
    public class ValidadeOutputDTO
    {
        public int IdValidade { get; set; }
        public string DataValidade { get; set; }
        public bool EmEstoque { get; set; }


        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }

    }
}

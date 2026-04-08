namespace Varejo.ViewModels
{
    public class ItemVinculoViewModel
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public string CodigoNoFornecedor { get; set; } // O campo que o usuário vai digitar
        public string DescricaoNoFornecedor { get; set; } // Opcional: xProd do fornecedor
    }
}

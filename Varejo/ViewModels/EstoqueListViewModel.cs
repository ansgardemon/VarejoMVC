namespace Varejo.ViewModels
{
    public class EstoqueListViewModel
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }

        public string Familia { get; set; }

        public decimal EstoqueAtual { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public decimal EstoqueMaximo { get; set; }

        public bool AbaixoMinimo { get; set; }
        public bool AcimaMaximo { get; set; }
    }
}

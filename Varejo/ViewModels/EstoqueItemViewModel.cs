namespace Varejo.ViewModels
{
    public class EstoqueItemViewModel
    {
        public int ProdutoId { get; set; }

        public string NomeProduto { get; set; }
        public string? Marca { get; set; }
        public string? Categoria { get; set; }

        public decimal EstoqueAtual { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public decimal EstoqueMaximo { get; set; }

        public decimal CustoMedio { get; set; }

        public bool AbaixoMinimo => EstoqueAtual < EstoqueMinimo;
        public bool AcimaMaximo => EstoqueAtual > EstoqueMaximo;
        public bool EstoqueZerado => EstoqueAtual == 0;
        public bool EstoqueNegativo => EstoqueAtual < 0;
    }
}

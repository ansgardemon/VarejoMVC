public class EstoqueViewModel
{
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; }

    public decimal EstoqueAtual { get; set; }

    public decimal EstoqueMinimo { get; set; }
    public decimal EstoqueMaximo { get; set; }

    public bool AbaixoDoMinimo => EstoqueAtual < EstoqueMinimo;
}
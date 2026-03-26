namespace Varejo.Models;

public class EstoqueConfig
{
    public int Id { get; set; }

    public int ProdutoId { get; set; }
    public Produto Produto { get; set; }

    public decimal EstoqueMinimo { get; set; }
    public decimal EstoqueMaximo { get; set; }
}

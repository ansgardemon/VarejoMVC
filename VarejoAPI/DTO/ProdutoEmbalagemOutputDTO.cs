public class ProdutoEmbalagemOutputDTO
{
    public int IdProdutoEmbalagem { get; set; }
    public int ProdutoId { get; set; }
    public int TipoEmbalagemId { get; set; }
    public decimal Preco { get; set; }
    public string? Ean { get; set; }
}

using VarejoAPI.DTO;

public class FamiliaOutputDTO
{
    public int IdFamilia { get; set; }
    public string NomeFamilia { get; set; }
    public List<ProdutoOutputDTO>? Produtos { get; internal set; }
}
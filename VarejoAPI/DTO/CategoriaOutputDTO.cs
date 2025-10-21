namespace VarejoAPI.DTO
{
    public class CategoriaOutputDTO
    {
        public int IdCategoria { get; set; }
        public string DescricaoCategoria { get; set; }
        public List<FamiliaOutputDTO> Familias { get; set; } = new();

    }
}

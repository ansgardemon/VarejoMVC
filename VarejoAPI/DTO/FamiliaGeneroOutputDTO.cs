namespace VarejoAPI.DTO
{
    public class FamiliaGeneroOutputDTO
    {

        public int IdFamilia { get; set; }
        public string NomeFamilia { get; set; }

        // para get categoria 
        public bool Ativo { get; set; }
        public string? CategoriaId { get; set; }
        public string? MarcaId { get; set; }

    }
}

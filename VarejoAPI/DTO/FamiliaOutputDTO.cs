using System.ComponentModel.DataAnnotations;
using Varejo.Models;
using VarejoAPI.DTO;

public class FamiliaOutputDTO
{
    public int IdFamilia { get; set; }
    public string NomeFamilia { get; set; }

    public bool Ativo { get; set; }
    public string? CategoriaId { get; set; }
    public string? MarcaId { get; set; }

}


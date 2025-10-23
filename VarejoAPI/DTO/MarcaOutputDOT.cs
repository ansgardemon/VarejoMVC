using System.ComponentModel.DataAnnotations;
using Varejo.Models;

namespace VarejoAPI.DTO
{
    public class MarcaOutputDOT
    {

        public int IdMarca { get; set; }
        [Required(ErrorMessage = "Nome da Categoria é obrigatoria")]
        public string NomeMarca { get; set; }        
    }
}

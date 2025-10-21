using System.ComponentModel.DataAnnotations;

namespace VarejoAPI.DTO
{
    public class CategoriaOutputDTO
    {

        [Required(ErrorMessage ="Nome da Categoria é obrigatoria")]
        public int IdCategoria { get; set; }


        [Required(ErrorMessage = "Nome da descrição é obrigatoria")]
        public string DescricaoCategoria { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace Varejo.ViewModels
{
    public class CategoriaViewModel
    {
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [Display(Name = "Categoria")]
        [StringLength(150)]
        public string DescricaoCategoria { get; set; }

        //número de produtos na categoria
        public int QuantidadeFamilia { get; set; }



    }
}

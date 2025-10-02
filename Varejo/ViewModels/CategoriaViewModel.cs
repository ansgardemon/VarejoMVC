using System.ComponentModel.DataAnnotations;
using Varejo.Models;

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



        //PARA PUXAR LISTA DETALHADA DAS FAMILIAS

        public List<FamiliaCategoriaViewModel>? Familias { get; set; }

    }
}

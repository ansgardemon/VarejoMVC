using System.ComponentModel.DataAnnotations;

namespace Varejo.ViewModels
{
    public class MarcaViewModel
    {


        [Key]
        public int IdMarca { get; set; }

        [Required]
        [StringLength(50)]
        public string NomeMarca { get; set; }


        //número de produtos na categoria
        public int QuantidadeFamilia { get; set; }


        //PARA PUXAR LISTA DETALHADA DAS FAMILIAS

        public List<FamiliaCategoriaViewModel> Familias { get; set; }


    }
}

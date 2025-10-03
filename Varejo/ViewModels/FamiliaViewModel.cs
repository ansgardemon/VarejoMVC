using System.ComponentModel.DataAnnotations;
using Varejo.Models;

namespace Varejo.ViewModels
{
    public class FamiliaViewModel
    {

        [Key]
        public int IdFamilia { get; set; }


        [Required]
        [StringLength(50)]
        public string NomeFamilia { get; set; }

        public bool Ativo { get; set; } = true;


        //RELACIONAMENTO COM OUTRAS TABELAS

        public int? MarcaId { get; set; }
        public virtual Marca? Marca { get; set; }

        public int CategoriaId { get; set; }
        public virtual Categoria? Categoria { get; set; }



        //PARA PUXAR OS PRODUTOS DENTRO DA FAMÍLIA

        public List<FamiliaDetailViewModel>? FamiliaDetails { get; set; }


    }
}

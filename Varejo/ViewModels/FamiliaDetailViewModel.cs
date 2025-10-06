using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Varejo.Models;

namespace Varejo.ViewModels
{
    public class FamiliaDetailViewModel
    {


        [Key]
        public int IdProduto { get; set; }


        [Required]
        [StringLength(101)]
        public string NomeProduto { get; set; }

        public bool Ativo { get; set; } = true;


        [StringLength(255)]
        [Required]
        public string UrlImagem { get; set; }


        //RELACIONAMENTO COM OUTRA TABELA

        public int FamiliaId { get; set; }
        public virtual Familia? Familia { get; set; }


    }
}

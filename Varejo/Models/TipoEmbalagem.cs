using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class TipoEmbalagem
    {
        [Key]
        public int IdTipoEmbalagem { get; set; }

        [Required]
        [StringLength(20)]
        public string DescricaoTipoEmbalagem { get; set; }


        [Required]
        public int Multiplicador { get; set; }



        //RELACIONAMENTO UM PARA MUITOS

        public virtual ICollection<ProdutoEmbalagem>? ProdutosEmbalagem { get; set; }


    }
}
using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class TipoEmbalagem
    {
        [Key]
        public int IdTipoEmbalagem { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Tipo Embalagem")]
        public string DescricaoTipoEmbalagem { get; set; }


        [Required]
        [Display(Name = "Multiplicador")]
        public int Multiplicador { get; set; }



        //RELACIONAMENTO UM PARA MUITOS
        [Display(Name = "Produtos Embalagem")]
        public virtual ICollection<ProdutoEmbalagem>? ProdutosEmbalagem { get; set; }


    }
}
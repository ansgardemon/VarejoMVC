using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Familia
    {


        [Key]
        public int IdFamilia { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "Família")]
        public string NomeFamilia { get; set; }

        public bool Ativo { get; set; } = true;


        //RELACIONAMENTO COM OUTRAS TABELAS

        [Display(Name = "Marca")]
        public int? MarcaId { get; set; }
        public virtual Marca? Marca { get; set; }

        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }
        public virtual Categoria? Categoria { get; set; }



        //RELACIONAMENTO UM PARA MUITOS
        [Display(Name = "Produtos")]
        public virtual ICollection<Produto>? Produtos { get; set; }

        [Display(Name = "Fornecedores")]
        public virtual ICollection<FornecedorFamilia>? FornecedoresFamilia { get; set; }


    }
}

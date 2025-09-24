using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Familia
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



        //RELACIONAMENTO UM PARA MUITOS

        public virtual ICollection<Produto>? Produtos { get; set; }


        public virtual ICollection<FornecedorFamilia>? FornecedorFamilias { get; set; }


    }
}

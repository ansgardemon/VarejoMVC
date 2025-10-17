using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class FornecedorFamilia
    {


        [Key]
        public int IdFornecedorFamilia { get; set; }


        //Relacionamento Com Outras Tabelas

        [Display(Name = "Fornecedor")]
        public int PessoaId { get; set; }
        public virtual Pessoa? Pessoa { get; set; }

        [Display(Name = "Família")]
        public int FamiliaId { get; set; }
        public virtual Familia? Familia { get; set; }
    }
}

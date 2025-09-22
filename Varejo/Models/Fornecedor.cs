using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Fornecedor
    {


        [Key]
        public int IdFornecedor { get; set; }



        //RELACIONAMENTO COM PESSOA
        public int PessoaId { get; set; }

        public virtual Pessoa? Pessoa { get; set; }





    }
}

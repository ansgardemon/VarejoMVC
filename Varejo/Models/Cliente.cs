using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Cliente
    {


        [Key]
        public int IdCliente { get; set; }



        //RELACIONAMENTO COM PESSOA
        public int PessoaId { get; set; }

        public virtual Pessoa? Pessoa { get; set; }

    }
}

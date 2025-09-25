using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Usuario
    {

        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(15)]
        public string nomeUsuario { get; set; }

        [StringLength(15)]
        [Required]
        public string Senha { get; set; }

        public bool Ativo { get; set; } = true;


        //RELACIONAMENTO COM PESSOA
        public int PessoaId { get; set; }

        public virtual Pessoa? Pessoa { get; set; }

        public int TipoUsuarioId { get; set; }

        public virtual TipoUsuario? TipoUsuario { get; set; }


    }
}

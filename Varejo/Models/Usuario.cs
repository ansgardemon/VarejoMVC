using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Usuario
    {

        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Máximo de 15 caraceteres.")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 80 caracteres.")]
        public string nomeUsuario { get; set; }

        [StringLength(15)]
        [Required(ErrorMessage ="Máximo de 15 caraceteres.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        public bool Ativo { get; set; } = true;


        //RELACIONAMENTO COM PESSOA
        public int PessoaId { get; set; }

        public virtual Pessoa? Pessoa { get; set; }

        public int TipoUsuarioId { get; set; }

        public virtual TipoUsuario? TipoUsuario { get; set; }


    }
}

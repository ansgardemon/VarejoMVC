using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Usuario
    {

        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Máximo de 15 caraceteres.")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 80 caracteres.")]
        [Display(Name = "Usuário")]
        public string nomeUsuario { get; set; }

        [StringLength(15)]
        [Required(ErrorMessage ="Máximo de 15 caraceteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;


        //RELACIONAMENTO COM PESSOA
        [Display(Name = "Pessoa")]
        public int PessoaId { get; set; }
        public virtual Pessoa? Pessoa { get; set; }

        [Display(Name = "Tipo Usuário")]
        public int TipoUsuarioId { get; set; }
        public virtual TipoUsuario? TipoUsuario { get; set; }


    }
}

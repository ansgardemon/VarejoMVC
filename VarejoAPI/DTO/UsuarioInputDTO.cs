using System.ComponentModel.DataAnnotations;

namespace VarejoAPI.DTO
{
    public class UsuarioInputDTO
    {
        [Required]
        public string nomeUsuario { get; set; }

        [Required]
        public string Senha { get; set; }

        public bool Ativo { get; set; }

        [Required]
        public int PessoaId { get; set; }

        [Required]
        public int TipoUsuarioId { get; set; }
    }
}

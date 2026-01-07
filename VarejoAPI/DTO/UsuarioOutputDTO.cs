namespace VarejoAPI.DTO
{
    public class UsuarioOutputDTO
    {
        public int IdUsuario { get; set; }
        public string nomeUsuario { get; set; }
        public bool Ativo { get; set; }
        public int PessoaId { get; set; }
        public int TipoUsuarioId { get; set; }
    }
}

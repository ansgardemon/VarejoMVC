namespace Varejo.Models
{

    public class UsuarioRelatorioFavorito
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int CodigoRelatorio { get; set; }

        // Relacionamento com a sua tabela de usuários existente
        public Usuario Usuario { get; set; }
    }
}
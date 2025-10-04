using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetAllAsync();
        Task<Usuario> GetByIdAsync(int id);
        Task<Usuario> GetAllAtivosAsync();
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task InativarUsuario(int id);
        Task ReativarUsuario(int id);
        //
        Task<Usuario>? ValidarLoginAsync(string email, string senha);

        Task<List<TipoUsuario>> GetTipoUsuarios();
        Task<List<Pessoa>> GetPessoa();

    }
}

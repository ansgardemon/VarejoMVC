using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly VarejoDbContext _context;
        public UsuarioRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> GetAllAtivosAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task InativarUsuario(int id)
        {
            throw new NotImplementedException();
        }

        public Task ReativarUsuario(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario>? ValidarLoginAsync(string email, string senha)
        {
            throw new NotImplementedException();
        }
    }
}

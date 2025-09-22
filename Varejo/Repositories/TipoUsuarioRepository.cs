using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class TipoUsuarioRepository : ITipoUsuarioRepository
    {
        private readonly VarejoDbContext _context;

        public TipoUsuarioRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(TipoUsuario tipoUsuario)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<TipoUsuario>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TipoUsuario> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TipoUsuario tipoUsuario)
        {
            throw new NotImplementedException();
        }
    }
}

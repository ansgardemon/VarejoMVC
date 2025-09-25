using Microsoft.EntityFrameworkCore;
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

        //CREATE
        public async Task AddAsync(TipoUsuario tipoUsuario)
        {
            await _context.TipoUsuarios.AddAsync(tipoUsuario);
            await _context.SaveChangesAsync();
        }

        //READ
        public Task<List<TipoUsuario>> GetAllAsync()
        {
            return _context.TipoUsuarios.ToListAsync();
        }

        //READE - ID
        public async Task<TipoUsuario> GetByIdAsync(int id)
        {
            return await _context.TipoUsuarios.FindAsync(id);
        }

        //UPDATE
        public async Task UpdateAsync(TipoUsuario tipoUsuario)
        {
            _context.TipoUsuarios.Update(tipoUsuario);
            await _context.SaveChangesAsync();
        }

        //DELETE
        public async Task DeleteAsync(int id)
        {
            var tipoUsuario = await _context.TipoUsuarios.FindAsync(id);
            if (tipoUsuario != null)
            {
                _context.TipoUsuarios.Remove(tipoUsuario);
                await _context.SaveChangesAsync();
            }
        }
    }
}

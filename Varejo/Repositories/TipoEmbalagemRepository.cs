using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class TipoEmbalagemRepository : ITipoEmbalagemRepository
    {
        private readonly VarejoDbContext _context;

        public TipoEmbalagemRepository(VarejoDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(TipoEmbalagem tipoEmbalagem)
        {
            await _context.TiposEmbalagem.AddAsync(tipoEmbalagem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tipo = await _context.TiposEmbalagem.FindAsync(id);
            if (tipo != null)
            {
                _context.TiposEmbalagem.Remove(tipo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<TipoEmbalagem>> GetAllAsync()
        {
            return await _context.TiposEmbalagem
                                 .ToListAsync();
        }

        public async Task<TipoEmbalagem?> GetByIdAsync(int id)
        {
            return await _context.TiposEmbalagem
                                 .Include(t => t.ProdutosEmbalagem) // opcional, para eager loading
                                 .FirstOrDefaultAsync(t => t.IdTipoEmbalagem == id);
        }

        public async Task UpdateAsync(TipoEmbalagem tipoEmbalagem)
        {
            _context.TiposEmbalagem.Update(tipoEmbalagem);
            await _context.SaveChangesAsync();
        }
    }
}

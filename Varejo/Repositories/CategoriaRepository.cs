using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly VarejoDbContext _context;

        public CategoriaRepository(VarejoDbContext context)
        {
            _context = context;
        }
        public async Task<List<Categoria>> GetAllAsync()
        {
            // Se quiser trazer as famílias junto, inclua: .Include(c => c.Familias)
            return await _context.Categorias
                               .Include(c => c.Familias)
                                 .ToListAsync();
        }

        public async Task<Categoria?> GetByIdAsync(int idCategoria)
        {
            return await _context.Categorias
                .Include(c => c.Familias)
                    .ThenInclude(f => f.Produtos)
                .FirstOrDefaultAsync(c => c.IdCategoria == idCategoria);
        }
        public async Task AddAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }
    }
}
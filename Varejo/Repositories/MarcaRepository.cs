using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class MarcaRepository : IMarcaRepository
    {
        private readonly VarejoDbContext _context;

        public MarcaRepository(VarejoDbContext context)
        {
            _context = context;
        }
        public async Task<List<Marca>> GetAllAsync()
        {
            // Se quiser trazer as famílias junto, inclua: .Include(c => c.Familias)
            return await _context.Marcas
                               .Include(c => c.Familias)
                                 .ToListAsync();
        }

        public async Task<Marca?> GetByIdAsync(int id)
        {
            return await _context.Marcas
                                 .Include(c => c.Familias) // inclui o relacionamento
                                 .FirstOrDefaultAsync(c => c.IdMarca == id);
        }

        public async Task AddAsync(Marca marca)
        {
            await _context.Marcas.AddAsync(marca);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Marca marca)
        {
            _context.Marcas.Update(marca);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Carrega a marca junto com suas famílias
            var marca = await _context.Marcas
                .Include(m => m.Familias)
                .FirstOrDefaultAsync(m => m.IdMarca == id);

            if (marca == null)
                return;

            // Se houver famílias associadas, lança exceção
            if (marca.Familias.Any())
                throw new InvalidOperationException("Não é possível deletar marca com famílias associadas.");

            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();
        }

    }
}

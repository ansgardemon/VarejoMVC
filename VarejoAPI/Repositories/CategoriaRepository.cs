using Varejo.Models;
using VarejoAPI.Interfaces;
using Varejo.Data;

namespace VarejoAPI.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly VarejoDbContext _context;
        public CategoriaRepository(VarejoDbContext context)
        {
            _context = context;
        }

        //CREATE
        public async Task AddAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        //UPDATE
        public async Task UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        //DELETE
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
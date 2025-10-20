using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly VarejoDbContext _context;

        public MovimentoRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Movimento>> GetAllAsync()
        {
            return await _context.Movimentos
                .Include(m => m.ProdutosMovimento)
                .ToListAsync();
        }

        public async Task<Movimento> GetByIdAsync(int id)
        {
            return await _context.Movimentos
                .Include(m => m.ProdutosMovimento)
                .FirstOrDefaultAsync(m => m.IdMovimento == id);
        }

        public async Task AddAsync(Movimento movimento)
        {
            _context.Movimentos.Add(movimento);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Movimento movimento)
        {
            _context.Movimentos.Update(movimento);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var movimento = await _context.Movimentos.FindAsync(id);
            if (movimento != null)
            {
                _context.Movimentos.Remove(movimento);
                await _context.SaveChangesAsync();
            }
        }
    }
}

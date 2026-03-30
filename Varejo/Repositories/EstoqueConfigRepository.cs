using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class EstoqueConfigRepository : IEstoqueConfigRepository
    {
        private readonly VarejoDbContext _context;

        public EstoqueConfigRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<EstoqueConfig?> GetByProdutoIdAsync(int produtoId)
        {
            return await _context.EstoquesConfig
                .FirstOrDefaultAsync(e => e.ProdutoId == produtoId);
        }

        public async Task<List<EstoqueConfig>> GetAllAsync()
        {
            return await _context.EstoquesConfig.ToListAsync();
        }

        public async Task AddAsync(EstoqueConfig config)
        {
            _context.EstoquesConfig.Add(config);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EstoqueConfig config)
        {
            _context.EstoquesConfig.Update(config);
            await _context.SaveChangesAsync();
        }
    }
}

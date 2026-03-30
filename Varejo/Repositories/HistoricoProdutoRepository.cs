using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;

namespace Varejo.Repositories
{
    public class HistoricoProdutoRepository : IHistoricoProdutoRepository
    {
        private readonly VarejoDbContext _context;

        public HistoricoProdutoRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(HistoricoProduto historico)
        {
            _context.HistoricosProduto.Add(historico);
            await _context.SaveChangesAsync();
        }

        public async Task<List<HistoricoProduto>> GetByProdutoIdAsync(int produtoId)
        {
            return await _context.HistoricosProduto
                .Where(h => h.ProdutoId == produtoId)
                .OrderBy(h => h.Data)
                .ToListAsync();
        }

        public async Task<List<HistoricoProduto>> GetByPeriodoAsync(DateTime inicio, DateTime fim)
        {
            return await _context.HistoricosProduto
                .Where(h => h.Data >= inicio && h.Data <= fim)
                .ToListAsync();
        }
    }
}

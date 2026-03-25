using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class PrazoPagamentoRepository : IPrazoPagamentoRepository
    {
        private readonly VarejoDbContext _context;

        public PrazoPagamentoRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<List<PrazoPagamento>> GetAllAsync()
        {
            return await _context.PrazosPagamento
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PrazoPagamento> GetByIdAsync(int id)
        {
            return await _context.PrazosPagamento.FindAsync(id);
        }

        public async Task AddAsync(PrazoPagamento prazo)
        {
            _context.PrazosPagamento.Add(prazo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PrazoPagamento prazo)
        {
            _context.PrazosPagamento.Update(prazo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var emUso = await _context.TitulosFinanceiro
                .AnyAsync(t => t.PrazoPagamentoId == id);

            if (emUso)
                throw new Exception("Prazo de pagamento está em uso.");

            var prazo = await GetByIdAsync(id);

            if (prazo != null)
            {
                _context.PrazosPagamento.Remove(prazo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string descricao)
        {
            return await _context.PrazosPagamento
                .AnyAsync(p => p.Descricao.ToLower() == descricao.ToLower());
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class FormaPagamentoRepository : IFormaPagamentoRepository
    {
        private readonly VarejoDbContext _context;

        public FormaPagamentoRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<List<FormaPagamento>> GetAllAsync()
        {
            return await _context.FormasPagamento
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FormaPagamento> GetByIdAsync(int id)
        {
            return await _context.FormasPagamento.FindAsync(id);
        }

        public async Task AddAsync(FormaPagamento forma)
        {
            _context.FormasPagamento.Add(forma);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FormaPagamento forma)
        {
            _context.FormasPagamento.Update(forma);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var emUso = await _context.TitulosFinanceiro
                .AnyAsync(t => t.FormaPagamentoId == id);

            if (emUso)
                throw new Exception("Forma de pagamento está em uso.");

            var forma = await GetByIdAsync(id);

            if (forma != null)
            {
                _context.FormasPagamento.Remove(forma);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string descricao)
        {
            return await _context.FormasPagamento
                .AnyAsync(f => f.DescricaoFormaPagamento.ToLower() == descricao.ToLower());
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class TipoMovimentoRepository : ITipoMovimentoRepository
    {
        private readonly VarejoDbContext _context;

        public TipoMovimentoRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<List<TipoMovimento>> GetAllAsync()
        {
            return await _context.TiposMovimento
                .Include(t => t.EspecieMovimento)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TipoMovimento?> GetByIdAsync(int id)
        {
            return await _context.TiposMovimento
                .Include(t => t.EspecieMovimento)
                .FirstOrDefaultAsync(t => t.IdTipoMovimento == id);
        }

        public async Task AddAsync(TipoMovimento tipoMovimento)
        {
            _context.TiposMovimento.Add(tipoMovimento);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TipoMovimento tipoMovimento)
        {
            _context.TiposMovimento.Update(tipoMovimento);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tipo = await _context.TiposMovimento.FindAsync(id);
            if (tipo != null)
            {
                _context.TiposMovimento.Remove(tipo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<EspecieMovimento>> GetAllEspeciesAsync()
        {
            return await _context.EspeciesMovimento
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

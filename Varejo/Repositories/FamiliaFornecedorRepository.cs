using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class FornecedorFamiliaRepository : IFamiliaFornecedorRepository
    {
        private readonly VarejoDbContext _context;

        public FornecedorFamiliaRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<List<FornecedorFamilia>> GetAllAsync()
        {
            return await _context.FornecedoresFamilia
                                 .Include(ff => ff.Pessoa)
                                 .Include(ff => ff.Familia)
                                 .ToListAsync();
        }

        public async Task<FornecedorFamilia?> GetByIdAsync(int id)
        {
            return await _context.FornecedoresFamilia
                                 .Include(ff => ff.Pessoa)
                                 .Include(ff => ff.Familia)
                                 .FirstOrDefaultAsync(ff => ff.IdFornecedorFamilia == id);
        }

        public async Task AddAsync(FornecedorFamilia fornecedorFamilia)
        {
            await _context.FornecedoresFamilia.AddAsync(fornecedorFamilia);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FornecedorFamilia fornecedorFamilia)
        {
            _context.FornecedoresFamilia.Update(fornecedorFamilia);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.FornecedoresFamilia.FindAsync(id);
            if (entity != null)
            {
                _context.FornecedoresFamilia.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<FornecedorFamilia>> GetByPessoaIdAsync(int pessoaId)
        {
            return await _context.FornecedoresFamilia
                                 .Include(ff => ff.Pessoa)
                                 .Include(ff => ff.Familia)
                                 .Where(ff => ff.PessoaId == pessoaId)
                                 .ToListAsync();
        }

        public async Task<List<FornecedorFamilia>> GetByFamiliaIdAsync(int familiaId)
        {
            return await _context.FornecedoresFamilia
                                 .Include(ff => ff.Pessoa)
                                 .Include(ff => ff.Familia)
                                 .Where(ff => ff.FamiliaId == familiaId)
                                 .ToListAsync();
        }
    }
}

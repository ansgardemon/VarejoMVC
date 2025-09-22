using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class PessoaRepository : IPessoaRepository
    {
        private readonly VarejoDbContext _context;

        public PessoaRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pessoa>> GetAllAsync()
        {
            return await _context.Pessoas.ToListAsync();
        }

        public async Task<Pessoa> GetByIdAsync(int id)
        {
            return await _context.Pessoas
                                 .FirstOrDefaultAsync(p => p.IdPessoa == id);
        }

        public async Task AddAsync(Pessoa pessoa)
        {
            await _context.Pessoas.AddAsync(pessoa);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pessoa pessoa)
        {
            _context.Pessoas.Update(pessoa);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa != null)
            {
                _context.Pessoas.Remove(pessoa);
                await _context.SaveChangesAsync();
            }
        }
    }
}

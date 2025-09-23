using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;


namespace Varejo.Repositories
{
    public class EnderecoRepository : IEnderecoRepository
    {
        private readonly VarejoDbContext _context;

        public EnderecoRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Endereco endereco)
        {
            await _context.AddAsync(endereco);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            var endereco = _context.Enderecos.Find(id);
            if (endereco != null)
            {
                _context.Enderecos.Remove(endereco);
                _context.SaveChanges();
            }
        }

        public async Task<List<Endereco>> GetAllAsync()
        {
            return await _context.Enderecos.ToListAsync();
        }

        public async Task<Endereco> GetByIdAsync(int id)
        {
            return await _context.Enderecos
                                 .FirstOrDefaultAsync(e => e.IdEndereco == id);
        }

        public Task UpdateAsync(Endereco endereco)
        {
            throw new NotImplementedException();
        }
    }
}

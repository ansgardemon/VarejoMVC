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

        //CREATE
        public async Task AddAsync(Endereco endereco)
        {
            await _context.AddAsync(endereco);
            await _context.SaveChangesAsync();
        }

        //READ
        public async Task<List<Endereco>> GetAllAsync()
        {
            return await _context.Enderecos.ToListAsync();
        }

        //READ - ID
        public async Task<Endereco> GetByIdAsync(int id)
        {
            return await _context.Enderecos
                                 .FirstOrDefaultAsync(e => e.IdEndereco == id);
        }

        //UPDATE
        public async Task UpdateAsync(Endereco endereco)
        {
            _context.Enderecos.Update(endereco);
            await _context.SaveChangesAsync();
        }

        //DELETE
        public async Task DeleteAsync(int id)
        {
            var endereco = _context.Enderecos.Find(id);
            if (endereco != null)
            {
                _context.Enderecos.Remove(endereco);
                _context.SaveChanges();
            }
        }
    }
}

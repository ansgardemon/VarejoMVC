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

        //CREATE
        public async Task AddAsync(Pessoa pessoa)
        {
            await _context.Pessoas.AddAsync(pessoa);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        //READ
        public async Task<List<Pessoa>> GetAllAsync()
        {
            return await _context.Pessoas.ToListAsync();

        }

        //READ - ID
        public async Task<Pessoa> GetByIdAsync(int id)
        {
            return await _context.Pessoas
          .Include(p => p.Enderecos) // garante que os endereços venham junto
          .FirstOrDefaultAsync(p => p.IdPessoa == id);
        }

        //UPDATE
        public async Task UpdateAsync(Pessoa pessoa)
        {
            _context.Pessoas.Update(pessoa);
            await _context.SaveChangesAsync();
        }



        //DELETE - OFF

        //SEARCH BY NAME

        public async Task<List<Pessoa>> SearchByNameAsync(string nome)
        {
            return await _context.Pessoas
                .Where(p => p.NomeRazao.Contains(nome))
                .AsNoTracking()
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<Pessoa>> GetClientesAtivosAsync()
        {
            return await _context.Pessoas
                .Where(p => p.EhCliente && p.Ativo)
                .OrderBy(p => p.NomeRazao)
                .AsNoTracking() // Melhora a performance em consultas apenas de leitura
                .ToListAsync();
        }

        public async Task<List<Pessoa>> SearchClientesAsync(string termo)
        {
            return await _context.Pessoas
                .Where(p => p.EhCliente && p.Ativo &&
                           (p.NomeRazao.Contains(termo) || p.CpfCnpj.Contains(termo)))
                .AsNoTracking()
                .Take(10)
                .ToListAsync();
        }
    }
}

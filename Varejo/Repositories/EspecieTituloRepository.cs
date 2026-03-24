using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class EspecieTituloRepository : IEspecieTituloRepository
    {
        private readonly VarejoDbContext _context;

        public EspecieTituloRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<List<EspecieTitulo>> GetAllAsync()
        {
            return await _context.EspeciesTitulo
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<EspecieTitulo> GetByIdAsync(int id)
        {
            return await _context.EspeciesTitulo
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdEspecieTitulo == id);
        }
    }
}
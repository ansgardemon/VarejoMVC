using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class ParametroRepository : IParametroRepository
    {
        private readonly VarejoDbContext _context;

        public ParametroRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<Parametro> GetAsync()
        {
            return await _context.Parametros
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task SaveAsync(Parametro parametro)
        {
            var existente = await _context.Parametros.FirstOrDefaultAsync();

            if (existente == null)
            {
                _context.Parametros.Add(parametro);
            }
            else
            {
                existente.TipoMovimentoVendaId = parametro.TipoMovimentoVendaId;
                existente.TipoMovimentoCompraId = parametro.TipoMovimentoCompraId;
                existente.TipoMovimentoAvariaId = parametro.TipoMovimentoAvariaId;
            }

            await _context.SaveChangesAsync();
        }
    }
}
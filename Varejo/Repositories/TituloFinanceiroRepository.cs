using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class TituloFinanceiroRepository : ITituloFinanceiroRepository
    {
        private readonly VarejoDbContext _context;

        public TituloFinanceiroRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<List<TituloFinanceiro>> GetAllAsync()
        {
            return await _context.TitulosFinanceiro
                .Include(t => t.EspecieTitulo)
                .Include(t => t.FormaPagamento)
                .Include(t => t.PrazoPagamento)
                .Include(t => t.Pessoa)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TituloFinanceiro> GetByIdAsync(int id)
        {
            return await _context.TitulosFinanceiro
                .Include(t => t.EspecieTitulo)
                .Include(t => t.FormaPagamento)
                .Include(t => t.PrazoPagamento)
                .Include(t => t.Pessoa)
                .FirstOrDefaultAsync(t => t.IdTituloFinanceiro == id);
        }

        public async Task<List<TituloFinanceiro>> GetByDocumentoAsync(int documento)
        {
            return await _context.TitulosFinanceiro
                .Where(t => t.Documento == documento)
                .OrderBy(t => t.Parcela)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(TituloFinanceiro titulo)
        {
            titulo.AtualizarValores();

            _context.TitulosFinanceiro.Add(titulo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TituloFinanceiro titulo)
        {
            titulo.AtualizarValores();

            _context.TitulosFinanceiro.Update(titulo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var titulo = await GetByIdAsync(id);

            if (titulo != null)
            {
                _context.TitulosFinanceiro.Remove(titulo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task BaixarTituloAsync(int id, decimal valorPago, DateTime dataPagamento)
        {
            var titulo = await GetByIdAsync(id);

            if (titulo == null)
                throw new Exception("Título não encontrado.");

            titulo.ValorPago = (titulo.ValorPago ?? 0) + valorPago;
            titulo.DataPagamento = dataPagamento;

            titulo.AtualizarValores();

            _context.TitulosFinanceiro.Update(titulo);
            await _context.SaveChangesAsync();
        }

        public async Task GerarTitulosAsync(
            int documento,
            decimal valorTotal,
            int prazoPagamentoId,
            int especieTituloId,
            int? formaPagamentoId,
            int? pessoaId,
            DateTime dataEmissao)
        {
            // vamos implementar depois com calma 👀
            throw new NotImplementedException();
        }
    }
}
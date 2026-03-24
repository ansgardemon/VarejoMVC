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
            var prazo = await _context.PrazosPagamento
                .FirstOrDefaultAsync(p => p.IdPrazoPagamento == prazoPagamentoId);

            if (prazo == null)
                throw new Exception("Prazo de pagamento não encontrado.");

            if (prazo.NumeroParcelas <= 0)
                throw new Exception("Prazo inválido.");

            var numeroParcelas = prazo.NumeroParcelas;
            var intervalo = prazo.IntervaloDias;

            // Divide valor
            var valorBase = Math.Round(valorTotal / numeroParcelas, 2);
            decimal soma = 0;

            var titulos = new List<TituloFinanceiro>();

            for (int i = 1; i <= numeroParcelas; i++)
            {
                var valorParcela = valorBase;

                // Ajuste de centavos na última parcela
                if (i == numeroParcelas)
                {
                    valorParcela = valorTotal - soma;
                }

                var titulo = new TituloFinanceiro
                {
                    Documento = documento,
                    Parcela = i,
                    Valor = valorParcela,
                    ValorPago = 0,
                    DataEmissao = dataEmissao,
                    DataVencimento = dataEmissao.AddDays(intervalo * i),
                    EspecieTituloId = especieTituloId,
                    FormaPagamentoId = formaPagamentoId,
                    PrazoPagamentoId = prazoPagamentoId,
                    PessoaId = pessoaId
                };

                titulo.AtualizarValores();

                titulos.Add(titulo);
                soma += valorParcela;
            }

            _context.TitulosFinanceiro.AddRange(titulos);
            await _context.SaveChangesAsync();
        }
    }
}
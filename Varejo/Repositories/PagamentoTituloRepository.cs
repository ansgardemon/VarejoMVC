using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Models;

public class PagamentoTituloRepository : IPagamentoTituloRepository
{
    private readonly VarejoDbContext _context;

    public PagamentoTituloRepository(VarejoDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PagamentoTitulo pagamento)
    {
        _context.PagamentosTitulo.Add(pagamento);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PagamentoTitulo>> GetByTituloIdAsync(int tituloId)
    {
        return await _context.PagamentosTitulo
            .Where(p => p.TituloFinanceiroId == tituloId)
            .OrderByDescending(p => p.DataPagamento)
            .ToListAsync();
    }

    public async Task RegistrarPagamentoAsync(int tituloId, decimal valor, DateTime data)
    {
        var titulo = await _context.TitulosFinanceiro
            .Include(t => t.Pagamentos)
            .FirstOrDefaultAsync(t => t.IdTituloFinanceiro == tituloId);

        if (titulo == null)
            throw new Exception("Título não encontrado.");

        var pagamento = new PagamentoTitulo
        {
            TituloFinanceiroId = tituloId,
            ValorPago = valor,
            DataPagamento = data
        };

        _context.PagamentosTitulo.Add(pagamento);

        // 🔥 NÃO adiciona manualmente na lista
        // titulo.Pagamentos.Add(pagamento); ❌ REMOVE ISSO

        titulo.AtualizarValores();

        _context.TitulosFinanceiro.Update(titulo);

        await _context.SaveChangesAsync();
    }
}
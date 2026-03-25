using Varejo.Models;

public interface IPagamentoTituloRepository
{
    Task AddAsync(PagamentoTitulo pagamento);
    Task<List<PagamentoTitulo>> GetByTituloIdAsync(int tituloId);

    Task RegistrarPagamentoAsync(int tituloId, decimal valor, DateTime data);
}
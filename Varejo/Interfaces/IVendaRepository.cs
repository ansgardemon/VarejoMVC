using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IVendaRepository
    {
        // Consultas
        Task<Venda?> GetByIdAsync(int id);
        Task<List<Venda>> GetAllAsync();

        // Persistência do Pedido
        Task<Venda> CriarPedidoAsync(Venda venda);
        Task<bool> AtualizarPedidoAsync(Venda venda);
        Task<bool> CancelarVendaAsync(int id);

        // O Coração: Transforma o Pedido em Movimento, Estoque e Financeiro
        Task<bool> FaturarVendaAsync(int vendaId);
    }
}
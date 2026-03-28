using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IInventarioRepository
    {
        Task<List<Inventario>> GetAllAsync();
        Task<Inventario?> GetByIdAsync(int id);

        Task DeleteAsync(int id);

        Task CriarInventarioAsync(Inventario inventario);
        Task FinalizarInventarioAsync(int inventarioId);

        Task AddItemAsync(InventarioItem item);

        Task AtualizarItemAsync(InventarioItem item);
        Task RemoverItemAsync(int itemId);
    }
}

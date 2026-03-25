using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IEspecieTituloRepository
    {
        Task<List<EspecieTitulo>> GetAllAsync();
        Task<EspecieTitulo> GetByIdAsync(int id);
    }
}
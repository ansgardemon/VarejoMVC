using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IFamiliaRepository
    {
        Task<List<Familia>> GetAllAsync();
        Task<Familia> GetByIdAsync(int id);
        Task AddAsync(Familia familia);
        Task UpdateAsync(Familia familia);
        Task DeleteAsync(int id);


        //MÉTODOS PARA DROPDOWN
        List<Marca> GetMarcas();
        List<Categoria> GetCategorias();

    }
}

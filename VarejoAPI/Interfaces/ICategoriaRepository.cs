using Varejo.Models;

namespace VarejoAPI.Interfaces
{
    public interface ICategoriaRepository
    {
        //CREATE
        Task AddAsync(Categoria categoria);

        //UPDATE
        Task UpdateAsync(Categoria categoria);

        //DELETE
        Task DeleteAsync(Categoria categoria);
    }
}

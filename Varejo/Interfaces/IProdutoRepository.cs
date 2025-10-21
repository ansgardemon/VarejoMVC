using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Interfaces
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> GetAllAsync();
        Task<Produto> GetByIdAsync(int id);

        Task<List<ProdutoViewModel>> GetByNameAsync(string query);

        Task<bool> ProdutoEmbalagemPossuiMovimentoAsync(int idProdutoEmbalagem);


        Task AddAsync(Produto produto);
        Task UpdateAsync(Produto produto);
        Task DeleteAsync(int id);


    }
}

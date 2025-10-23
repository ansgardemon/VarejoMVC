using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Interfaces
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> GetAllAsync();
        Task<Produto> GetByIdAsync(int id);

        Task<List<ProdutoViewModel>> GetByNameAsync(string query);

        Task<List<Produto>> GetByFamilia(int id);
        Task<bool> ProdutoEmbalagemPossuiMovimentoAsync(int idProdutoEmbalagem);

        Task<List<Produto>> GetProdutosDestaqueAsync(int take = 8);

        Task<List<Produto>> GetProdutosCatalogoAsync(int? idCategoria, int? idMarca);


        Task AddAsync(Produto produto);
        Task UpdateAsync(Produto produto);
        Task DeleteAsync(int id);


    }
}

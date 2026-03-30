using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Interfaces
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> GetAllAsync();

        Task<List<Produto>> GetAllDetailedAsync();

        Task<Produto> GetByIdAsync(int id);

        Task<Produto> GetByIdDetailedAsync(int id);

        Task<List<ProdutoViewModel>> GetByNameAsync(string query);

        Task<IEnumerable<Produto>> SearchAsync(string query);

        Task<List<Produto>> GetByFamilia(int id);
        Task<List<Produto>> GetByCategory(int id);
        Task<bool> ProdutoEmbalagemPossuiMovimentoAsync(int idProdutoEmbalagem);

        Task<List<Produto>> GetProdutosDestaqueAsync(int take = 8);

        Task<List<Produto>> GetProdutosCatalogoAsync(int? idCategoria, int? idMarca);

        Task<List<EstoqueListViewModel>> GetEstoqueAsync(EstoqueFiltroViewModel filtro);

        Task<List<HistoricoProduto>> GetHistoricoAsync(int produtoId, DateTime? inicio, DateTime? fim);

        Task AddAsync(Produto produto);
        Task UpdateAsync(Produto produto);
        Task DeleteAsync(int id);


    }
}

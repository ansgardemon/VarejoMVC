using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IFornecedorConfigRepository
    {
        // --- Gestão de Famílias do Fornecedor ---
        Task<IEnumerable<Familia>> GetFamiliasPorFornecedorAsync(int pessoaId);
        Task<bool> VincularFamiliaAsync(int pessoaId, int familiaId);
        Task<bool> RemoverFamiliaAsync(int pessoaId, int familiaId);

        // --- Gestão de Vínculo de Produtos (De-Para) ---
        Task<IEnumerable<ProdutoFornecedorVinculo>> GetVinculosPorFornecedorAsync(int pessoaId);

        // Busca se aquele código <cProd> do XML já existe para aquele fornecedor
        Task<ProdutoFornecedorVinculo> GetVinculoPorCodigoExternoAsync(int pessoaId, string codigoExterno);

        Task<bool> SalvarVinculoProdutoAsync(ProdutoFornecedorVinculo vinculo);
    }
}

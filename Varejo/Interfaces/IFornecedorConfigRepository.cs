using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Interfaces
{

 
        public interface IFornecedorConfigRepository
        {
            // --- Gestão de Famílias ---
            Task<IEnumerable<Familia>> GetFamiliasPorFornecedorAsync(int pessoaId);
            Task<bool> VincularFamiliaAsync(int pessoaId, int familiaId);
            Task<bool> RemoverFamiliaAsync(int pessoaId, int familiaId);

            // --- Gestão de Vínculos (De-Para) ---
            Task<IEnumerable<ProdutoFornecedorVinculo>> GetVinculosPorFornecedorAsync(int pessoaId);
            Task<ProdutoFornecedorVinculo> GetVinculoPorCodigoExternoAsync(int pessoaId, string codigoExterno);

            // O método de "salvamento em massa" que você quer usar na View
            Task<bool> SalvarLoteVinculosAsync(int pessoaId, List<ItemVinculoViewModel> itens);

            // Backup para salvar um vínculo avulso
            Task<bool> SalvarVinculoProdutoAsync(ProdutoFornecedorVinculo vinculo);
        }
   
}

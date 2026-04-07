using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IEmpresaRepository
    {
        Task<ConfiguracaoEmpresa> GetConfiguracaoAsync();
        Task<bool> SalvarConfiguracaoAsync(ConfiguracaoEmpresa config);
    }
}

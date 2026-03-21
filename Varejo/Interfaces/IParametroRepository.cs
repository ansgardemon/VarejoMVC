using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IParametroRepository
    {
        Task<Parametro> GetAsync(); // único registro
        Task SaveAsync(Parametro parametro);
    }
}
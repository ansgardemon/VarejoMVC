using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories;

    public class EspecieTituloRepository : IEspecieTituloRepository
{
    public Task<List<EspecieTitulo>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<EspecieTitulo> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}


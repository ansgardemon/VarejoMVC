using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IEstoqueSnapshotRepository
    {
        Task GerarSnapshotAsync();
        Task<List<EstoqueSnapshot>> GetUltimoSnapshotAsync();
    }
}

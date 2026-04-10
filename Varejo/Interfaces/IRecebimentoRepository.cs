using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IRecebimentoRepository
    {
        Task<Recebimento> GetByIdAsync(int id);
        Task<IEnumerable<Recebimento>> GetAllAsync();

        // O principal: Salva a nota, os itens e deve orquestrar a Transaction
        Task<bool> RegistrarRecebimentoAsync(Recebimento recebimento);

        // Para evitar duplicidade no XML
        Task<bool> ExisteChaveAcessoAsync(string chaveAcesso);

        Task<bool> DesintegrarRecebimentoAsync(int id);
    }
}

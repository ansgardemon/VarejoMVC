using Varejo.ViewModels;

namespace Varejo.Interfaces
{
    public interface IXmlService
    {
        // Retorna um objeto com os dados do cabeçalho e a lista de itens parseada
        RevisaoNotaViewModel CarregarDadosDoXml(IFormFile arquivo);
    }
}
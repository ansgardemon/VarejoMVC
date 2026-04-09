using Microsoft.AspNetCore.Mvc.Rendering;

namespace Varejo.ViewModels
{
    public class ItemRevisaoViewModel
    {
        public string CodigoFornecedor { get; set; }
        public string NomeProdutoXml { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }

        // O usuário vai selecionar estes no "De-Para" da tela
        public int? ProdutoIdInterno { get; set; }
        public int? ProdutoEmbalagemId { get; set; } // ESSENCIAL para o estoque

        public bool EhBonificacao { get; set; }

        public List<SelectListItem> EmbalagensDisponiveis { get; set; } = new List<SelectListItem>();
    }
}

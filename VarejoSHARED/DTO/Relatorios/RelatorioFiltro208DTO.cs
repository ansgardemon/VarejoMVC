using VarejoSHARED.DTO.Relatorios;

namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltro208DTO : RelatorioFiltroProdutosDTO
    {
        public bool OcultarEstoqueSistema { get; set; } = true;
    }
    public class Relatorio208DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal EstoqueAtual { get; set; }
    }
}
namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltro205DTO : RelatorioFiltroProdutosDTO
    {
        public bool OcultarEstoqueSistema { get; set; } = true;
        public bool ApenasAbaixoDoMinimo { get; set; } = false;
    }

    public class Relatorio205DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal EstoqueAtual { get; set; }
        public decimal EstoqueMinimo { get; set; }
    }
}
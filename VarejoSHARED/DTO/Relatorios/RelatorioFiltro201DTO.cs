namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltro201DTO : RelatorioFiltroProdutosDTO
    {
        // "TODOS", "COM_ESTOQUE", "SEM_ESTOQUE", "NEGATIVO"
        public string FiltroSaldo { get; set; } = "COM_ESTOQUE";
    }

    public class Relatorio201DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public decimal EstoqueAtual { get; set; }
        public decimal CustoMedio { get; set; }

        // Propriedade calculada na hora
        public decimal ValorTotalCusto => EstoqueAtual * CustoMedio;
    }
}
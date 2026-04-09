namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltro206DTO : RelatorioFiltroProdutosDTO
    {
        // Se o usuário quiser ver apenas produtos com estoque para não sujar o relatório
        public bool ApenasComEstoque { get; set; } = true;
    }

    public class Relatorio206DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal EstoqueAtual { get; set; }

        public decimal CustoMedio { get; set; }
        public decimal PrecoVenda { get; set; } // Pegaremos da embalagem principal

        // Propriedades Financeiras Calculadas
        public decimal ValorTotalCusto => EstoqueAtual > 0 ? EstoqueAtual * CustoMedio : 0;
        public decimal ValorTotalVenda => EstoqueAtual > 0 ? EstoqueAtual * PrecoVenda : 0;
        public decimal LucroProjetado => ValorTotalVenda - ValorTotalCusto;

        // Evita divisão por zero
        public decimal MargemProjetada => ValorTotalVenda > 0 ? (LucroProjetado / ValorTotalVenda) * 100 : 0;
    }
}
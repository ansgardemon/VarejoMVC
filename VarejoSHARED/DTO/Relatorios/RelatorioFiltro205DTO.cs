namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltro205DTO : RelatorioFiltroProdutosDTO
    {
        // Filtro para ver apenas quem está abaixo do mínimo ou todos que possuem mínimo cadastrado
        public bool ApenasAbaixoDoMinimo { get; set; } = true;
    }

    public class Relatorio205DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal EstoqueAtual { get; set; }
        public decimal EstoqueMinimo { get; set; }

        // Quanto falta para atingir o mínimo
        public decimal NecessidadeCompra => EstoqueMinimo > EstoqueAtual ? EstoqueMinimo - EstoqueAtual : 0;
    }
}
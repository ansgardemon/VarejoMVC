namespace VarejoSHARED.DTO
{
    public class Relatorio104DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal QuantidadeVendida { get; set; }
        public decimal Faturamento { get; set; }
        public decimal PercentualAcumulado { get; set; }
        public string ClasseABC { get; set; } = string.Empty; // Retornará "A", "B" ou "C"
    }
}
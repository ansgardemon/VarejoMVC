namespace VarejoSHARED.DTO
{
    public class Relatorio102DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Embalagem { get; set; } = string.Empty; // Ex: Fardo c/ 12
        public decimal CustoMedio { get; set; }
        public decimal PrecoVenda { get; set; }
        public decimal MargemBruta => PrecoVenda > 0 ? ((PrecoVenda - CustoMedio) / PrecoVenda) * 100 : 0;
        public decimal Markup => CustoMedio > 0 ? ((PrecoVenda - CustoMedio) / CustoMedio) * 100 : 0;
    }
}
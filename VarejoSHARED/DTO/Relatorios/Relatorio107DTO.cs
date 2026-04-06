namespace VarejoSHARED.DTO
{
    public class Relatorio107DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime DataAlteracao { get; set; }
        public decimal PrecoAnterior { get; set; }
        public decimal PrecoNovo { get; set; }
        public string Usuario { get; set; } = string.Empty;

        // Propriedade calculada (Não precisa vir do banco)
        public decimal VariacaoPercentual =>
            PrecoAnterior > 0 ? ((PrecoNovo - PrecoAnterior) / PrecoAnterior) * 100 : 0;
    }
}
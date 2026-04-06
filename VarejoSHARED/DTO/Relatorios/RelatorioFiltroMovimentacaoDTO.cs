namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltroMovimentacaoDTO : RelatorioFiltroBaseDTO
    {
        public int? IdPessoa { get; set; }
        public string? TipoMovimento { get; set; }
    }
}
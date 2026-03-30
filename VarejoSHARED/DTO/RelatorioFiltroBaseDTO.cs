namespace VarejoSHARED.DTO
{
    public class RelatorioFiltroBaseDTO
    {
        public int CodigoRelatorio { get; set; }
        public string? TermoBusca { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }
}
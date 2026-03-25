namespace VarejoSHARED.DTO
{
    public class FiltroRelatorioDTO
    {
        // 📅 Período
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }

        // 🔎 Busca genérica
        public string? Busca { get; set; }

        // 📊 Ordenação
        public string? CampoOrdenacao { get; set; }
        public string? Direcao { get; set; } = "desc";

        // 📄 Paginação
        public int Pagina { get; set; } = 1;
        public int ItensPorPagina { get; set; } = 10;
    }
}
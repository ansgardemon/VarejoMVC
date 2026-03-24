namespace Varejo.Models.UI
{
    public class RelatorioColuna<T>
    {
        public string Titulo { get; set; } = "";
        public string Campo { get; set; } = "";
        public Func<T, object> Selector { get; set; } = default!;
    }
}

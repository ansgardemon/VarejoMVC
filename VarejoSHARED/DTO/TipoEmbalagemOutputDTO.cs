namespace VarejoSHARED.DTO
{
    public class TipoEmbalagemOutputDTO
    {
        public int TipoEmbalagemId { get; set; }

        public string DescricaoTipoEmbalagem { get; set; } = string.Empty;

        public int Multiplicador { get; set; }

        public int ProdutoEmbalagemId { get; set; }
    }
}

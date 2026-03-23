using Varejo.Models;

namespace VarejoSHARED.DTO
{
    public class TipoEmbalagemInputDTO
    {
        public string DescricaoTipoEmbalagem { get; set; } = string.Empty;

        public int Multiplicador { get; set; }

        public int ProdutoEmbalagemId { get; set; }
     
    }
}

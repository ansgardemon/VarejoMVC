using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarejoSHARED.DTO
{
    public class InventarioItemOutputDTO
    {

        public int Id { get; set; }
        public int InventarioId { get; set; }
        public int ProdutoId { get; set; }
        public string? NomeProduto { get; set; }
        public int ProdutoEmbalagemId { get; set; }
        public string? DescricaoEmbalagem { get; set; }
        public int Multiplicador { get; set; }
        public decimal QuantidadeSistema { get; set; }
        public decimal QuantidadeContada { get; set; }
        public decimal Diferenca { get; set; }
    }
}

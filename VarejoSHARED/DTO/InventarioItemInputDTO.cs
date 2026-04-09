using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarejoSHARED.DTO
{
    public class InventarioItemInputDTO
    {
        public int InventarioId { get; set; }
        public int ProdutoId { get; set; }
        public int ProdutoEmbalagemId { get; set; }
        public decimal QuantidadeContada { get; set; }
    }
}

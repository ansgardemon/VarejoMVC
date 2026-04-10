using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarejoSHARED.DTO
{
    public class VendaItemInputDTO
    {
        public int ProdutoId { get; set; }
        public int ProdutoEmbalagemId { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal DescontoUnitario { get; set; }
    }
}

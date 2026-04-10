using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarejoSHARED.DTO
{
    public class VendaItemOutputDTO
    {
        public int IdVendaItem { get; set; }

        public int ProdutoId { get; set; }
        public string? NomeProduto { get; set; }

        public int ProdutoEmbalagemId { get; set; }
        public string? NomeEmbalagem { get; set; }

        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal DescontoUnitario { get; set; }
        public decimal Subtotal { get; set; }

    }
}

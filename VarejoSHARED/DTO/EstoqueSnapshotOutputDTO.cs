using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarejoSHARED.DTO
{
    public class EstoqueSnapshotOutputDTO
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public DateTime Data { get; set; }
        public decimal Estoque { get; set; }

    }
}

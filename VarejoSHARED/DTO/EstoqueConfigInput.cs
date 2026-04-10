using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarejoSHARED.DTO
{
    public class EstoqueConfigInput
    {

        public int ProdutoId { get; set; } 
        public decimal EstoqueMinimo { get; set; }
        public decimal EstoqueMaximo { get; set; }

    }
}

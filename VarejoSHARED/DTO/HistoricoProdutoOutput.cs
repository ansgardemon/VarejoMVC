using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarejoSHARED.DTO
{
    public class HistoricoProdutoOutput
    {

        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string? TipoMovimento { get; set; }
        public string? EspecieMovimento { get; set; }
        public decimal EstoqueAntes { get; set; }
        public decimal QuantidadeMovimento { get; set; }
        public decimal EstoqueDepois { get; set; }
        public string? Observacao { get; set; }



    }
}

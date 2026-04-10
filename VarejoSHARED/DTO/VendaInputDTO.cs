using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarejoSHARED.DTO
{
    public class VendaInputDTO
    {


        public int PessoaId { get; set; }
        public int FormaPagamentoId { get; set; }
        public int? PrazoPagamentoId { get; set; }
        public string? Observacao { get; set; }
        public decimal DescontoTotal { get; set; }
        public List<VendaItemInputDTO> Itens { get; set; } = new();

    }
}

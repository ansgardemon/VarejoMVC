using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarejoSHARED.DTO
{
    public class InventarioOutputDTO
    {

        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string? Observacao { get; set; }
        public bool Finalizado { get; set; }
        public List<InventarioItemOutputDTO> Itens { get; set; } = new();


    }
}

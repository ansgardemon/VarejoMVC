using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class PrazoPagamento
    {

        [Key]
 
        public int IdPrazoPagamento { get; set; }

        [Required]
        public string Descricao { get; set; }

        public int NumeroParcelas { get; set; }

        public int IntervaloDias { get; set; }



        // RELACIONAMENTOS
        public ICollection<TituloFinanceiro> Titulos { get; set; }




    }
}

using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Prazo
    {
        [Key]
        public int IdPrazo { get; set; }

        [Required]
        public string DescricaoPrazo { get; set; }

        [Required]
        public int NumeroParcela { get; set; } //Define em quantas parcelas o pagamento será dividido

        [Required]
        public int Dias { get; set; } //Define a quantidade de dias para o vencimento da parcela


    }
}

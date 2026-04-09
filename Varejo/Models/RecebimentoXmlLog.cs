using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class RecebimentoXmlLog
    {
        [Key]
        public int IdXmlLog { get; set; }

        [Required, StringLength(44)]
        public string ChaveAcesso { get; set; } // Chave única da NF-e

        public DateTime DataImportacao { get; set; } = DateTime.Now;

        public int RecebimentoId { get; set; } // FK para o recebimento gerado
        public virtual Recebimento Recebimento { get; set; }

        public string XmlCaminho { get; set; } // Opcional: Caminho onde o arquivo foi salvo
    }
}

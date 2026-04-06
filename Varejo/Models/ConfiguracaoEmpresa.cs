using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class ConfiguracaoEmpresa
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string RazaoSocial { get; set; }

        [Required, StringLength(18)]
        public string Cnpj { get; set; } // O sistema comparará com a tag <dest><CNPJ> do XML

        [StringLength(100)]
        public string NomeFantasia { get; set; }

        public string Logotipo { get; set; } // Path para o logo se quiser usar no Cupom depois
    }
}

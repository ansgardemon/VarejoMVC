using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Pessoa
    {

        [Key]
        public int IdPessoa { get; set; }

        [Required]
        [StringLength(100)]
        public string NomeRazao { get; set; }


        [StringLength(100)]
        [Required]
        public string TratamentoFantasia { get; set; }

        [StringLength(18)]
        [Required]
        public string CpfCnpj { get; set; }


        [StringLength(2)]
        public string Ddd { get; set; }

        public string Telefone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        public bool EhJuridico { get; set; }

        public bool EhUsuario { get; set; }

        public bool EhCliente { get; set; }

        public bool EhFornecedor { get; set; }

        public bool Ativo { get; set; } = true;





    }
}

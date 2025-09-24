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

        public bool EhJuridico { get; set; } = false;

        public bool EhUsuario { get; set; } = false;

        public bool EhCliente { get; set; } = false;

        public bool EhFornecedor { get; set; } = false;

        public bool Ativo { get; set; } = true;



        //RELACIONAMENTO UM PARA MUITOS
        public virtual ICollection<Endereco>? Enderecos { get; set; }





    }
}

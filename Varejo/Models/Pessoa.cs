using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Pessoa
    {

        [Key]
        public int IdPessoa { get; set; }

        [Required]
        [StringLength(100)]

        [Display(Name = "Nome / Razão")]
        public string NomeRazao { get; set; }

        [StringLength(100)]
        [Required]
        [Display(Name = "Tratamento / Fantasia")]
        public string TratamentoFantasia { get; set; }

        [StringLength(18)]
        [Required]
        [Display(Name = "CPF / CNPJ")]
        public string CpfCnpj { get; set; }

        [StringLength(2)]
        [Display(Name = "DDD")]
        public string Ddd { get; set; }


        [Phone]
        public string Telefone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }


        [Display(Name = "Jurídico")]
        public bool EhJuridico { get; set; } = false;


        [Display(Name = "Usuário")]
        public bool EhUsuario { get; set; } = false;


        [Display(Name = "Cliente")]
        public bool EhCliente { get; set; } = false;


        [Display(Name = "Jurídico")]
        public bool EhFornecedor { get; set; } = false;

        public bool Ativo { get; set; } = true;



        //RELACIONAMENTO UM PARA MUITOS
        public virtual ICollection<Endereco>? Enderecos { get; set; }





    }
}

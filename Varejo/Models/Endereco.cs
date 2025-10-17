using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Endereco
    {
        /// <summary>
    
        /// </summary>

        [Key]
        public int IdEndereco { get; set; }

        [StringLength(150)]
        [Required]
        [Display(Name ="Logradouro")]
        public string Logradouro { get; set; }

        [StringLength(9)]
        [Required]
        [Display(Name = "CEP")]
        public string Cep { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name ="Bairro")]
        public string Bairro { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name ="Cidade")]
        public string Cidade { get; set; }

        [StringLength(2)]
        [Required]
        [Display(Name = "UF")]
        public string Uf { get; set; }

        [StringLength(100)]
        [Display(Name = "Complemento")]
        public string Complemento { get; set; }

        [StringLength(10)]
        [Display(Name = "Número")]
        public string Numero { get; set; }



        //RELACIONAMENTO COM PESSOA
        [Display(Name = "Pessoa")]
        public int PessoaId { get; set; }
        public virtual Pessoa? Pessoa { get; set; }
    }
}

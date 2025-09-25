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
        public string Logradouro { get; set; }

        [StringLength(9)]
        [Required]
        public string Cep { get; set; }

        [StringLength(50)]
        [Required]
        public string Bairro { get; set; }

        [StringLength(50)]
        [Required]
        public string Cidade { get; set; }

        [StringLength(2)]
        [Required]
        public string Uf { get; set; }

        [StringLength(100)]
        public string Complemento { get; set; }

        [StringLength(10)]
        public string Numero { get; set; }



        //RELACIONAMENTO COM PESSOA
        public int PessoaId { get; set; }

        public virtual Pessoa? Pessoa { get; set; }
    }
}

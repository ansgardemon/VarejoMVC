using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Varejo.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Varejo.ViewModels
{
    public class ProdutoViewModel
    {
        [Key]
        public int IdProduto { get; set; }

        [Required]
        [StringLength(50)]
        public string Complemento { get; set; }

        [StringLength(101)]
        public string NomeProduto { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal EstoqueInicial { get; set; } = 0;

        public bool Ativo { get; set; } = true;


        [StringLength(255)]
        public string UrlImagem { get; set; }

        public IFormFile? ImagemUpload { get; set; }


        [Column(TypeName = "decimal(18,4)")]
        public decimal CustoMedio { get; set; }


        //RELACIONAMENTO COM OUTRA TABELA

        public int FamiliaId { get; set; }
        public virtual Familia? Familia { get; set; }




    }
}

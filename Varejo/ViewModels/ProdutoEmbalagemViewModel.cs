using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Varejo.ViewModels
{
    public class ProdutoEmbalagemViewModel
    {
        public int IdProdutoEmbalagem { get; set; }

        [Required(ErrorMessage = "O preço do produto é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O código EAN é obrigatório.")]
        public string Ean { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public int TipoEmbalagemId { get; set; }

        // Para popular o dropdown
        public IEnumerable<SelectListItem>? TiposEmbalagem { get; set; }


    }
}

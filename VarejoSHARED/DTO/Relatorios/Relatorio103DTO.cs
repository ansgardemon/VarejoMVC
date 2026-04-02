// Caminho: VarejoSHARED/DTO/Relatorio103DTO.cs
using System;

namespace VarejoSHARED.DTO.Relatorios
{
    public class Relatorio103DTO
    {
        public int IdMovimento { get; set; }
        public DateTime DataMovimento { get; set; }

        public string TipoMovimento { get; set; } = string.Empty;
        public string Pessoa { get; set; } = string.Empty;

        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; } = string.Empty;

        public decimal Quantidade { get; set; }
        public string Embalagem { get; set; } = string.Empty;
    }
}
// Caminho: VarejoSHARED/DTO/RelatorioFiltro103DTO.cs
using System.Collections.Generic;

namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltro103DTO : RelatorioFiltroBaseDTO
    {
        // A regra exige que seja possível selecionar mais de um produto
        public List<int> ProdutosIds { get; set; } = new();
    }
}
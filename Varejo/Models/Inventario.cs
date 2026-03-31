namespace Varejo.Models
{
    public class Inventario
    {
        public int Id { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        public string? Observacao { get; set; }

        public bool Finalizado { get; set; }

        public ICollection<InventarioItem> Itens { get; set; } = new List<InventarioItem>();
    }
}

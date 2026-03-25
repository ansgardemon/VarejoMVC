namespace VarejoAPI.DTO
{
    public class MovimentoOutputDTO
    {
        public int IdMovimento { get; set; }

        public int? Documento { get; set; }

        public string Observacao { get; set; }

        public DateTime DataMovimento { get; set; }

        public string Pessoa { get; set; }

        public string TipoMovimento { get; set; }

        public List<ProdutoMovimentoOutputDTO> Produtos { get; set; }

    }
}

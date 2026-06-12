namespace KoridrawsPI.Models.DTOs.Pedido
{
    public class GetPedidosDto
    {
        public int Id { get; set; }
        public DateTime DataEmissao { get; set; }
        public string ClienteNome { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Pagamento { get; set; } = string.Empty;
        public string FreteServico { get; set; } = string.Empty;
        public decimal FreteValor { get; set; }
        public string? CodigoRastreio { get; set; }
        public int TotalItens { get; set; }
        public string EnderecoCompleto { get; set; } = string.Empty;

        public List<PedidoItensDto> Itens { get; set; } = new();

        public class PedidoItensDto
        {
            public int ItemId { get; set; }
            public string NomeProduto { get; set; } = string.Empty;
            public int Quantidade { get; set; }
            public decimal PrecoUnitario { get; set; }
        }
    }

 
}

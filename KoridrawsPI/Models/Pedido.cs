using System.Text.Json.Serialization;

namespace KoridrawsPI.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public int EnderecoId { get; set; }
        public Endereco? Endereco { get; set; }
        public ICollection<PedidoItem> ItensPedido { get; set; } = new List<PedidoItem>();
        public decimal ValorTotal { get; set; }
        public StatusPedido Status { get; set; } = StatusPedido.Criado;
        public MetodoPagamento Pagamento { get; set; }
        public Frete? Frete { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StatusPedido
    {
        Criado,
        AguardandoPagamento,
        PagamentoConfirmado,
        EmPreparacao,
        EmEnvio,
        Concluido,
        Cancelado
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MetodoPagamento
    {
        CartaoCredito,
        CartaoDebito,
        Boleto,
        Pix
    }
}
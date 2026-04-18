using System.Text.Json.Serialization;

namespace KoridrawsPI.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime DataEmissao { get; set; } = DateTime.UtcNow;

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public ICollection<Item> Itens { get; set; } = new List<Item>();
        public StatusPedido Status { get; set; } = StatusPedido.Criado;
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
}

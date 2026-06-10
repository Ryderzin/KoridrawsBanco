using KoridrawsPI.Models;

public class Frete
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }
    public string Servico { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int PrazoDias { get; set; }
    public string? CodigoRastreio { get; set; }
}
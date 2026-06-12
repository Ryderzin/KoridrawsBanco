using System.ComponentModel.DataAnnotations;

namespace KoridrawsPI.Models.DTOs.Pedido
{
    public class PedidoCriacaoDto
    {
        [Required]
        public int EnderecoId { get; set; }

        [Required]
        public MetodoPagamento Pagamento { get; set; }

        [Required]
        public List<PedidoItemDto> Itens { get; set; } = new();

        [Required]
        public FreteCriacaoDto Frete { get; set; } = new();
    }

    public class PedidoItemDto
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        public int Quantidade { get; set; }
    }

    public class FreteCriacaoDto
    {
        [Required]
        public string Servico { get; set; } = string.Empty;

        [Required]
        public decimal Valor { get; set; }

        [Required]
        public int PrazoDias { get; set; }
    }
}
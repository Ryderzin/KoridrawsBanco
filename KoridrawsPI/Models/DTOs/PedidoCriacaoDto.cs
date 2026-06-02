using System.ComponentModel.DataAnnotations;

namespace KoridrawsPI.Models.DTOs
{
    public class PedidoCriacaoDto
    {
        [Required]
        public int EnderecoId { get; set; }

        [Required]
        public MetodoPagamento Pagamento { get; set; }

        [Required]
        public List<PedidoItemCriacaoDto> Itens { get; set; } = new List<PedidoItemCriacaoDto>();
    }

    public class PedidoItemCriacaoDto
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        public int Quantidade { get; set; }
    }
}
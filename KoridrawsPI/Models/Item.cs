using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KoridrawsPI.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        public int Estoque { get; set; }

        public int? GerenteId { get; set; }

        [ForeignKey("GerenteId")]
        public Gerente? Gerente { get; set; }

        [JsonIgnore]
        public ICollection<PedidoItem> ItensPedido { get; set; } = new List<PedidoItem>();

        public ICollection<Imagem> Imagens { get; set; } = new List<Imagem>();
    }
}
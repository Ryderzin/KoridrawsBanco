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
        public double Preco { get; set; }
        public int? GerenteId { get; set; }

        [ForeignKey("GerenteId")]
        public Gerente? Gerente { get; set; }

        [JsonIgnore]
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public ICollection<Imagem> Imagens { get; set; } = new List<Imagem>();
    }
}

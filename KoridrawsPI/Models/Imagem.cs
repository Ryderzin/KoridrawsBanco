using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KoridrawsPI.Models
{
    public class Imagem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Url { get; set; } = string.Empty;
        public string? CaminhoCloud { get; set; }
        public int? ItemId { get; set; }

        [JsonIgnore]
        [ForeignKey("ItemId")]
        public Item? Item { get; set; }
    }
}

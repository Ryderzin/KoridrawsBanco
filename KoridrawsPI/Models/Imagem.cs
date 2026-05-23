using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KoridrawsPI.Models
{
    public class Imagem
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? CaminhoCloud { get; set; }

        public int? ItemId { get; set; }

        [JsonIgnore]
        public Item? Item { get; set; }

        public int? EventoId { get; set; }

        [JsonIgnore]
        public Evento? Evento { get; set; }
    }
}

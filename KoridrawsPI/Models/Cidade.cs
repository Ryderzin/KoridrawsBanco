using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KoridrawsPI.Models
{
    public class Cidade
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int EstadoId { get; set; }
        public Estado? Estado { get; set; }
        [JsonIgnore]
        public ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();
    }
}
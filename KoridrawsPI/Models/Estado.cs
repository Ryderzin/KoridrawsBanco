using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KoridrawsPI.Models
{
    public class Estado
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        [JsonIgnore]
        public ICollection<Cidade> Cidades { get; set; } = new List<Cidade>();
    }
}
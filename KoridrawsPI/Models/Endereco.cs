using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KoridrawsPI.Models
{
    public class Endereco
    {
        public int Id { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Numero { get; set; } = string.Empty;
        public int CidadeId { get; set; }
        public Cidade? Cidade { get; set; }

        public int? ClienteId { get; set; }

        [JsonIgnore]
        public Cliente? Cliente { get; set; }


    }
}

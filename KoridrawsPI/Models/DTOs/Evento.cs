using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoridrawsPI.Models.DTOs
{
    public class Evento
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public DateTime Data { get; set; }

        public int EnderecoId { get; set; }

        [ForeignKey("EnderecoId")]
        public Endereco? Endereco { get; set; }

        public int? GerenteId { get; set; }

        [ForeignKey("GerenteId")]
        public Gerente? Gerente { get; set; }

        public ICollection<Imagem> Imagens { get; set; } = new List<Imagem>();
    }
}

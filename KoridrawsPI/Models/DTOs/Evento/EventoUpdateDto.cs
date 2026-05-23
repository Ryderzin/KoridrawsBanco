using System.ComponentModel.DataAnnotations;

namespace KoridrawsPI.Models
{
    public class EventoUpdateDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        [Required]
        public DateTime Data { get; set; }

        [Required]
        public int EnderecoId { get; set; }

        public List<int> ImagensParaRemover { get; set; } = new List<int>();

        public List<IFormFile> NovasImagens { get; set; } = new List<IFormFile>();
    }
}

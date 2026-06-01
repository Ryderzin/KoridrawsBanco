using System.ComponentModel.DataAnnotations;

namespace KoridrawsPI.Models
{
    public class EnderecoDto
    {
        [Required]
        public string Rua { get; set; } = string.Empty;

        [Required]
        public string Numero { get; set; } = string.Empty;

        public string Bairro { get; set; } = string.Empty;

        public string Cep { get; set; } = string.Empty;

        public string? Complemento { get; set; }

        [Required]
        public int CidadeId { get; set; }
    }

}
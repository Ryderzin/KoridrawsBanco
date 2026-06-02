using System.ComponentModel.DataAnnotations;

namespace KoridrawsPI.Models.DTOs.Item
{
    public class ItemUploadDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public decimal Preco { get; set; }

        public List<IFormFile> Imagens { get; set; } = new List<IFormFile>();
    }
}
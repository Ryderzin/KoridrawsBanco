using KoridrawsPI.Models.DTOs.ImagemDtos;
using System.ComponentModel.DataAnnotations;

namespace KoridrawsPI.Models.DTOs.Item
{
    public class GetItemDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        public int Estoque { get; set; }

        public ICollection<ImagemRelationDto> Imagens { get; set; } = new List<ImagemRelationDto>();
    }
}


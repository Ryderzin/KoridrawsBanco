using KoridrawsPI.Models.DTOs.ImagemDtos;

namespace KoridrawsPI.Models.DTOs.Item
{
    public class GetItensDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public ImagemRelationDto? Imagem { get; set; }
    }
}

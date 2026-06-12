using KoridrawsPI.Models.DTOs.Endereco;
using KoridrawsPI.Models.DTOs.ImagemDtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoridrawsPI.Models.DTOs.Evento
{
    public class GetEventosDto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public DateTime Data { get; set; }

        public int EnderecoId { get; set; }

        public EnderecoEventoDto? Endereco { get; set; }

        public ImagemRelationDto? Imagem { get; set; } = new ImagemRelationDto();
    }
}

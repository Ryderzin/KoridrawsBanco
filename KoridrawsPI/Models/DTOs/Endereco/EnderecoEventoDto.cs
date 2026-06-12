namespace KoridrawsPI.Models.DTOs.Endereco
{
    public class EnderecoEventoDto
    {
        public int Id { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string CidadeNome { get; set; } = string.Empty;
    }
}

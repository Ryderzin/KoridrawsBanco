namespace KoridrawsPI.Models.DTOs
{
    public class PerfilCompletoDto
    {
        public PerfilUsuarioDto Perfil { get; set; } = new();
        public List<PerfilEnderecoDto> Enderecos { get; set; } = new();
        public List<PerfilPedidoDto> Pedidos { get; set; } = new();
    }

    public class PerfilUsuarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Papel { get; set; } = string.Empty;
    }

    public class PerfilEnderecoDto
    {
        public int Id { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Numero { get; set; } = string.Empty;
        public Cidade? Cidade { get; set; }

    }

    public class PerfilPedidoDto
    {
        public int Id { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Pagamento { get; set; } = string.Empty;
        public string EnderecoEntregaResumido { get; set; } = string.Empty;
        public List<PerfilPedidoItemDto> Itens { get; set; } = new();
        public PerfilPedidoFreteDto Frete { get; set; } = new();
    }

    public class PerfilPedidoItemDto
    {
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }

    public class PerfilPedidoFreteDto
    {
        public string Servico { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public int PrazoDias { get; set; }
    }
}
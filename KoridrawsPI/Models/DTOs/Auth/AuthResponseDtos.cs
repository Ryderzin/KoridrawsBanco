namespace KoridrawsPI.Models.DTOs.Auth
{
    public class AuthResponseDtos
    {

        public class LoginResponseDto
        {
            public string Token { get; set; } = string.Empty;
            public DateTime Expiracao { get; set; }
            public UsuarioResponseDto Usuario { get; set; } = new();
        }

        public class RegistroResponseDto
        {
            public string Mensagem { get; set; } = string.Empty;
            public UsuarioResponseDto Usuario { get; set; } = new();
        }

        public class UsuarioResponseDto
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Papel { get; set; } = string.Empty;
        }
    }
}

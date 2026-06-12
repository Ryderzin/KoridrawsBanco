using System.ComponentModel.DataAnnotations;

namespace KoridrawsPI.Models.DTOs.Auth
{
    public class PasswordResetDto
    {
        public class EsqueciSenhaDto
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public class ResetarSenhaDto
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Token { get; set; } = string.Empty;

            [Required, MinLength(6, ErrorMessage = "A nova senha deve ter pelo menos 6 caracteres.")]
            public string NovaSenha { get; set; } = string.Empty;
        }
    }
}

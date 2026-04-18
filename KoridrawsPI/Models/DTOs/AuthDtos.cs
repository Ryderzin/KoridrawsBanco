using System.ComponentModel.DataAnnotations;

namespace KoridrawsPI.Models.DTOs
{
    public class AuthDtos
    {
        public class RegistroDto
        {
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "A senha é obrigatória.")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
            public string Senha { get; set; } = string.Empty;

            [Required(ErrorMessage = "O perfil (Cliente ou Gerente) é obrigatório.")]
            public string Perfil { get; set; } = string.Empty; // "Cliente" ou "Gerente"

            // Aqui você pode adicionar campos extras como "Nome", "Setor" (se for gerente), etc., 
            // para já criar a entidade Cliente/Gerente junto com o registro do usuário.
            public string Nome { get; set; } = string.Empty;
        }

        public class LoginDto
        {
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "A senha é obrigatória.")]
            public string Senha { get; set; } = string.Empty;
        }
    }
}

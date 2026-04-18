using System.ComponentModel.DataAnnotations;

namespace KoridrawsPI.Models.Abstract
{
    public abstract class Usuario
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string SenhaHash { get; set; } = string.Empty;

        [Required]
        public string Papel { get; set; } = string.Empty; // "Cliente" ou "Gerente"
    }
}

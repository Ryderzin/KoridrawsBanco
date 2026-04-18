using KoridrawsPI.Models.Abstract;
using System.ComponentModel.DataAnnotations;

namespace KoridrawsPI.Models
{
    public class Gerente : Usuario
    {
        public string Setor { get; set; } = string.Empty;
    }
}

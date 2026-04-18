using KoridrawsPI.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KoridrawsPI.Models
{
    public class Cliente : Usuario
    {
        public int? ImagemPerfilId { get; set; }
        public Imagem? ImagemPerfil { get; set; }

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();

    }
}

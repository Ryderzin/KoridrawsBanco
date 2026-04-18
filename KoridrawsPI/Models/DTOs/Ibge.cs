using System.Text.Json.Serialization;

namespace KoridrawsPI.Models.DTOs
{
    public class Ibge
    {
        public class IbgeEstadoDto
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("sigla")]
            public string Sigla { get; set; } = string.Empty;

            [JsonPropertyName("nome")]
            public string Nome { get; set; } = string.Empty;
        }

        public class IbgeCidadeDto
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("nome")]
            public string Nome { get; set; } = string.Empty;

            [JsonPropertyName("microrregiao")]
            public MicrorregiaoDto Microrregiao { get; set; } = new();
        }

        public class MicrorregiaoDto
        {
            [JsonPropertyName("mesorregiao")]
            public MesorregiaoDto Mesorregiao { get; set; } = new();
        }

        public class MesorregiaoDto
        {
            [JsonPropertyName("UF")]
            public IbgeEstadoDto Uf { get; set; } = new();
        }
    }
}


using System.Text.Json.Serialization;

namespace KoridrawsPI.Models.DTOs
{
    public class FreteDto
    {
        public class CalculoFreteRequestDto
        {
            public string CepOrigem { get; set; } = string.Empty;
            public string CepDestino { get; set; } = string.Empty;
        }

        public class OpcaoFreteDto
        {
            public int ServicoId { get; set; }
            public string Servico { get; set; } = string.Empty;
            public decimal Valor { get; set; }
            public int PrazoDias { get; set; }
            public string Transportadora { get; set; } = string.Empty;
        }

        public class MelhorEnvioResponse
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("custom_price")]
            public string CustomPrice { get; set; } = string.Empty;

            [JsonPropertyName("custom_delivery_time")]
            public int CustomDeliveryTime { get; set; }

            [JsonPropertyName("company")]
            public MelhorEnvioCompany? Company { get; set; }

            [JsonPropertyName("error")]
            public string? Error { get; set; }
        }

        public class MelhorEnvioCompany
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
        }
    }
}

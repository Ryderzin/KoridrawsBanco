using KoridrawsPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using static KoridrawsPI.Models.DTOs.FreteDto;

namespace KoridrawsPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreteController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public FreteController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("calcular")]
        public async Task<ActionResult<List<OpcaoFreteDto>>> CalcularFrete([FromForm] CalculoFreteRequestDto freteDto)
        {
            if (string.IsNullOrWhiteSpace(freteDto.CepDestino) || string.IsNullOrWhiteSpace(freteDto.CepOrigem))
            {
                return BadRequest("Os CEPs de origem e destino são obrigatórios.");
            }

            var payload = new
            {
                from = new { postal_code = freteDto.CepOrigem },
                to = new { postal_code = freteDto.CepDestino },
                products = new[]
                {
                    new
                    {
                        id = "pedido_1",
                        weight = 0.5,
                        width = 10,
                        height = 10,
                        length = 10,
                        insurance_value = 0
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();

            string urlApi = "https://melhorenvio.com.br/api/v2/me/shipment/calculate";
            string token = _configuration["MelhorEnvio:Token"];

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            client.DefaultRequestHeaders.Add("User-Agent", "SuaLoja (seu_email@dominio.com)");

            var response = await client.PostAsync(urlApi, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return StatusCode(502, $"Erro na API do Melhor Envio. Status: {response.StatusCode}. Detalhe: {errorBody}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var cotacoesMelhorEnvio = JsonSerializer.Deserialize<List<MelhorEnvioResponse>>(jsonResponse);

            if (cotacoesMelhorEnvio == null)
            {
                return StatusCode(500, "Erro ao processar os dados do Melhor Envio.");
            }

            var opcoesDeFrete = cotacoesMelhorEnvio
                .Where(c => string.IsNullOrEmpty(c.Error))
                .Select(c => new OpcaoFreteDto
                {
                    ServicoId = c.Id,
                    Servico = c.Name,
                    Transportadora = c.Company?.Name ?? "Desconhecida",
                    Valor = decimal.Parse(c.CustomPrice, System.Globalization.CultureInfo.InvariantCulture),
                    PrazoDias = c.CustomDeliveryTime
                })
                .OrderBy(c => c.Valor)
                .ToList();

            return Ok(opcoesDeFrete);
        }
    }
}
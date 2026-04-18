using KoridrawsPI.Data;
using KoridrawsPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static KoridrawsPI.Models.DTOs.Ibge;

namespace KoridrawsPI.Seeders
{
    public static class DbSeeder
    {
        public static async Task SeedIbgeDataAsync(Context context)
        {
            // Verifica se os estados já foram inseridos para não duplicar
            if (context.Estados.Any()) return;

            using var httpClient = new HttpClient();

            // Busca os Estados
            var estadosResponse = await httpClient.GetStringAsync("https://servicodados.ibge.gov.br/api/v1/localidades/estados");
            var estadosIbge = JsonSerializer.Deserialize<List<IbgeEstadoDto>>(estadosResponse);

            if (estadosIbge != null)
            {
                var estados = estadosIbge.Select(e => new Estado
                {
                    Id = e.Id,
                    Descricao = e.Nome,
                    Sigla = e.Sigla
                }).ToList();
                await context.Estados.AddRangeAsync(estados);
                await context.SaveChangesAsync();
                // Iniciamos uma transação para poder ligar/desligar o IDENTITY_INSERT
                //using var transaction = await context.Database.BeginTransactionAsync();
                //try
                //{
                //    // Habilita a inserção manual para a tabela ESTADO
                //    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Estados ON");

         

                //    // Desabilita após o uso
                //    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Estados OFF");

                //    await transaction.CommitAsync();
                //}
                //catch (Exception)
                //{
                //    await transaction.RollbackAsync();
                //    throw;
                //}
            }

            // 2. Busca as Cidades
            var cidadesResponse = await httpClient.GetStringAsync("https://servicodados.ibge.gov.br/api/v1/localidades/municipios");

            // SOLUÇÃO 1: Configurar o desserializador para não ligar para maiúsculas/minúsculas
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var cidadesIbge = JsonSerializer.Deserialize<List<IbgeCidadeDto>>(cidadesResponse, jsonOptions);

            if (cidadesIbge != null)
            {
                var cidades = cidadesIbge
                    // SOLUÇÃO 2: Filtro de segurança (só processa se toda a cadeia existir)
                    .Where(c => c.Microrregiao?.Mesorregiao?.Uf != null)
                    .Select(c => new Cidade
                    {
                        Descricao = c.Nome,
                        // Como passamos pelo Where acima, temos certeza que não será nulo aqui
                        EstadoId = c.Microrregiao!.Mesorregiao!.Uf!.Id
                    }).ToList();

                await context.Cidades.AddRangeAsync(cidades);
                await context.SaveChangesAsync();
            }
        }
    }
}
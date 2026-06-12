using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Text.Json;

namespace KoridrawsPI.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _service;
        private readonly string _folderId = "1v6sGvy2gHUgqSuFBFXy6lLppahBXoHHr";

        public GoogleDriveService(IConfiguration configuration)
        {
            string jsonCredenciais = configuration["GoogleDrive:CredentialsJson"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(jsonCredenciais))
            {
                throw new Exception("As credenciais nao foram encontradas nas variaveis de ambiente.");
            }

            var dados = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonCredenciais);

            if (dados == null || !dados.ContainsKey("private_key") || !dados.ContainsKey("client_email"))
            {
                throw new Exception("Chaves obrigatorias ausentes no JSON de credenciais.");
            }

            string privateKey = dados["private_key"].Replace("\\n", "\n");
            string clientEmail = dados["client_email"];

            var credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(clientEmail)
                {
                    Scopes = new[] { DriveService.ScopeConstants.Drive }
                }.FromPrivateKey(privateKey));

            _service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "KoridrawsPI"
            });
        }

        public async Task<(string Url, string CaminhoCloud)> UploadImagemAsync(IFormFile arquivo)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = arquivo.FileName,
                Parents = new List<string> { _folderId }
            };

            FilesResource.CreateMediaUpload request;

            using (var memoryStream = new MemoryStream())
            {
                await arquivo.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                request = _service.Files.Create(fileMetadata, memoryStream, arquivo.ContentType);
                request.Fields = "id, webViewLink";

                var progresso = await request.UploadAsync();

                if (progresso.Status == Google.Apis.Upload.UploadStatus.Failed)
                {
                    throw new Exception(progresso.Exception?.Message);
                }
            }

            var file = request.ResponseBody;

            if (file == null)
            {
                throw new Exception("Upload falhou.");
            }

            return (file.WebViewLink, file.Id);
        }

        public async Task ExcluirImagemAsync(string fileId)
        {
            await _service.Files.Delete(fileId).ExecuteAsync();
        }
    }
}
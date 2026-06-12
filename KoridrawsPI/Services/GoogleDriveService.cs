using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Text;

namespace KoridrawsPI.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _service;
        private readonly string _folderId = "1v6sGvy2gHUgqSuFBFXy6lLppahBXoHHr";

        public GoogleDriveService(IConfiguration configuration)
        {
            string base64Credenciais = configuration["GoogleDrive:CredentialsBase64"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(base64Credenciais))
            {
                throw new Exception("As credenciais não foram encontradas.");
            }

            byte[] bytes = Convert.FromBase64String(base64Credenciais);
            string jsonCredenciais = Encoding.UTF8.GetString(bytes);

            var credential = GoogleCredential.FromJson(jsonCredenciais)
                .CreateScoped(new[] { DriveService.ScopeConstants.Drive });

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
                request.SupportsAllDrives = true;
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
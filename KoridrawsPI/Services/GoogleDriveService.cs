using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace KoridrawsPI.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _service;
        private readonly string _folderId = "1v6sGvy2gHUgqSuFBFXy6lLppahBXoHHr";

        public GoogleDriveService()
        {
            GoogleCredential credential;

            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(new[] { DriveService.ScopeConstants.Drive });
            }

            _service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SuaLojaApi"
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
            try
            {
                await _service.Files.Delete(fileId).ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
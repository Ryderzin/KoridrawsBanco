using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace KoridrawsPI.Services
{
    public class GoogleDriveService
    {
        private readonly string _credentialsPath = "client_secret.json";
        private readonly string _folderId = "1v6sGvy2gHUgqSuFBFXy6lLppahBXoHHr";
        private readonly string[] Scopes = { DriveService.Scope.DriveFile };

        private async Task<DriveService> ObterServicoAsync()
        {
            UserCredential credential;

            using (var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SuaLojaApi"
            });
        }

        public async Task<(string Url, string CaminhoCloud)> UploadImagemAsync(IFormFile arquivo)
        {
            var service = await ObterServicoAsync();

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

                request = service.Files.Create(fileMetadata, memoryStream, arquivo.ContentType);
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
                throw new Exception("Upload falhou e o ResponseBody retornou null.");
            }

            return (file.WebViewLink, file.Id);
        }

        public async Task ExcluirImagemAsync(string fileId)
        {
            var service = await ObterServicoAsync();

            try
            {
                await service.Files.Delete(fileId).ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KoridrawsPI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string HeaderName = "X-Admin-Key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var extractedKey))
            {
                context.Result = new UnauthorizedObjectResult("Acesso negado. Chave de API não fornecida.");
                return;
            }

            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>("AdminApiKey");

            if (!apiKey.Equals(extractedKey))
            {
                context.Result = new UnauthorizedObjectResult("Acesso negado. Chave de API inválida.");
                return;
            }

            await next();
        }
    }
}
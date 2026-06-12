using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SuaLojaApi.Swagger
{
    public class AdminKeyHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IOpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "dmin-Key",
                In = ParameterLocation.Header,
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String
                }
            });
        }
    }
}
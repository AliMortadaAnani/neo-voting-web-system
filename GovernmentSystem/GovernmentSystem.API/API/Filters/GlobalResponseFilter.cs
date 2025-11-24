using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GovernmentSystem.API.API.Filters
{
    public class GlobalResponseFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // 1. Always document 500 Server Error
            if (!operation.Responses.ContainsKey("500"))
            {
                operation.Responses.Add("500", new OpenApiResponse { Description = "Internal Server Error (ProblemDetails)" });
            }

            // 2. Always document 400 Bad Request (Validation)
            if (!operation.Responses.ContainsKey("400"))
            {
                operation.Responses.Add("400", new OpenApiResponse { Description = "Validation Error (ValidationProblemDetails)" });
            }

            // 3. Document 401/403 only if the endpoint is protected
            // (Swagger checks for [Authorize] or Security Requirements)
            if (operation.Security != null && operation.Security.Count > 0)
            {
                if (!operation.Responses.ContainsKey("401"))
                    operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            }
        }
    }
}
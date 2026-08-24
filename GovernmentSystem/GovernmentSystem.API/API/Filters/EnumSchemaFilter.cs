using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GovernmentSystem.API.API.Filters
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();

                // Maps both the Name and the Numeric value into the Swagger documentation schema
                Enum.GetValues(context.Type)
                    .Cast<object>()
                    .ToList()
                    .ForEach(enumValue =>
                    {
                        var name = Enum.GetName(context.Type, enumValue);
                        var numericValue = (int)enumValue;

                        // Formats it neatly as "Name (Value)" or just injects the descriptive text
                        schema.Enum.Add(new OpenApiString($"{name} = {numericValue}"));
                    });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Auth;

public class ConfirmationHeadersSwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
        {
            return;
        }

        var hasAttrOnMethod = actionDescriptor
            .MethodInfo.GetCustomAttributes(typeof(RequireConfirmationAttribute), true)
            .Length != 0;

        if (!hasAttrOnMethod)
        {
            return;
        }

        operation.Parameters ??= new List<OpenApiParameter>();

        var headers = new[] { "X-Confirmation-Id", "X-Confirmation-Code", "X-Confirmation-Method" };

        foreach (var header in headers)
        {
            operation.Parameters.Add(
                new OpenApiParameter
                {
                    Name = header,
                    In = ParameterLocation.Header,
                    Schema = new OpenApiSchema { Type = "string" }
                }
            );
        }
    }
}

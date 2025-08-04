using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireConfirmationAttribute(
    ConfirmableAction action,
    ConfirmationMethod confirmationMethod
) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext ctx,
        ActionExecutionDelegate next
    )
    {
        var httpCtx = ctx.HttpContext;

        if (
            !httpCtx.Request.Headers.TryGetValue("X-Confirmation-Code", out var code)
            || !httpCtx.Request.Headers.TryGetValue("X-Confirmation-Id", out var id)
            || !httpCtx.Request.Headers.TryGetValue("X-Confirmation-Method", out var method)
        )
        {
            var res = ApiResponse.Custom(
                401,
                new
                {
                    message = "Confirmation required",
                    allowedMethods = new[] { confirmationMethod.ToString() },
                }
            );
            await ApiResponse.ApplyAsync(httpCtx, res);
            return;
        }

        if (
            !Guid.TryParse(id, out var parsedId)
            || !Enum.TryParse<ConfirmationMethod>(method, out var parsedMethod)
        )
        {
            var res = ApiResponse.BadRequest("Invalid confirmation id or method");
            await ApiResponse.ApplyAsync(httpCtx, res);
            return;
        } // parsedMethod will be useful in the future

        if (
            !httpCtx.Items.TryGetValue(AuthCtxConstants.AuthUser, out var value)
            || value is not AuthorizedUser authUser
        )
        {
            throw new InvalidOperationException(
                $"{nameof(RequireConfirmationAttribute)} cannot be used without authorization."
            );
        }

        var mediator = httpCtx.RequestServices.GetRequiredService<IMediator>();

        var cmd = new ConfirmActionCommand(
            authUser.UserId,
            code!,
            action,
            confirmationMethod,
            parsedId
        );
        var result = await mediator.Send(cmd);

        if (result.IsSuccess)
        {
            await next();
            return;
        }

        var apiError = result.Exception switch
        {
            NoSuch or ConfirmationMismatch => ApiResponse.NotFound(
                "Code assigned to given confirmation id, method and action was not found"
            ),
            InvalidConfirmationCode => ApiResponse.Unauthorized("Invalid code"),
            Expired => ApiResponse.Timeout("Given code has already expired"),
            _ => throw result.Exception,
        };

        await ApiResponse.ApplyAsync(httpCtx, apiError);
    }
}

using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Other;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireConfirmationAttribute(
    ConfirmationMethod confirmationMethod,
    ConfirmableAction action
) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext ctx,
        ActionExecutionDelegate next
    )
    {
        var httpCtx = ctx.HttpContext;

        if (!httpCtx.Request.Headers.TryGetValue("X-Confirmation-Code", out var code))
        {
            var res = ApiResponse.Custom(
                401,
                new
                {
                    message = "Confirmation required",
                    allowedMethod = confirmationMethod.ToString(),
                }
            );
            await ApiResponse.ApplyAsync(httpCtx, res);
            return;
        }

        if (
            !httpCtx.Items.TryGetValue("authorizedUser", out var value)
            || value is not AuthorizedUser authUser
        )
        {
            throw new InvalidOperationException(
                $"{nameof(RequireConfirmationAttribute)} cannot be used without authorization."
            );
        }

        var mediator = httpCtx.RequestServices.GetRequiredService<IMediator>();

        var cmd = new ConfirmActionCommand(authUser.UserId, code!, action, confirmationMethod);
        var result = await mediator.Send(cmd);

        if (result.IsSuccess)
        {
            await next();
            return;
        }

        var apiError = result.Exception switch
        {
            NoSuch => ApiResponse.Unauthorized("Invalid code"),
            Expired => ApiResponse.Timeout("Given code has already expired"),
            _ => throw result.Exception,
        };

        await ApiResponse.ApplyAsync(httpCtx, apiError);
    }
}

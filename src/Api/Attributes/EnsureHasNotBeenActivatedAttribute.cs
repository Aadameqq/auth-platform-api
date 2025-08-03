using Api.Auth;
using Core.Queries.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class EnsureHasNotBeenActivatedAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext ctx,
        ActionExecutionDelegate next
    )
    {
        var httpCtx = ctx.HttpContext;
        if (
            !httpCtx.Items.TryGetValue("authorizedUser", out var value)
            || value is not AuthorizedUser authUser
        )
        {
            throw new InvalidOperationException(
                $"{nameof(EnsureHasNotBeenActivatedAttribute)} cannot be used without authorization."
            );
        }

        var mediator = httpCtx.RequestServices.GetRequiredService<IMediator>();

        var result = await mediator.Send(new HasAccountBeenActivatedQuery(authUser.UserId));

        if (result.Value)
        {
            await ApiResponse.ApplyAsync(httpCtx, ApiResponse.Forbid("Account already activated"));
            return;
        }

        await next();
    }
}

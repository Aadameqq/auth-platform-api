using Core.Domain;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Auth;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireAnyRoleAttribute(params Role[] roles) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext ctx,
        ActionExecutionDelegate next
    )
    {
        var httpCtx = ctx.HttpContext;
        if (
            !httpCtx.Items.TryGetValue(AuthCtxConstants.AuthUser, out var value)
            || value is not AuthorizedUser authUser
        )
        {
            throw new InvalidOperationException(
                $"{nameof(RequireAuthAttribute)} cannot be used without authorization."
            );
        }

        var hasAny = roles.Any(role => role == authUser.Role);

        if (!hasAny)
        {
            await ApiResponse.ApplyAsync(httpCtx, ApiResponse.Forbid());
            return;
        }

        await next();
    }
}

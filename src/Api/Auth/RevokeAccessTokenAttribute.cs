using Core.Ports;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RevokeAccessTokenAttribute : Attribute, IAsyncActionFilter
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
                $"{nameof(RevokeAccessTokenAttribute)} cannot be used without authorization."
            );
        }

        var revokedTokensRepository =
            httpCtx.RequestServices.GetRequiredService<RevokedTokensRepository>();
        await revokedTokensRepository.Revoke(authUser.Token, authUser.LifeTimeLeft);

        await next();
    }
}

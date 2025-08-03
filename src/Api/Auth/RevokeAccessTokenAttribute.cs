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
        if (httpCtx.Items.TryGetValue("accessToken", out var value) && value is string token)
        {
            var revokedTokensRepository =
                httpCtx.RequestServices.GetRequiredService<RevokedTokensRepository>();
            await revokedTokensRepository.Revoke(token);
        }

        await next();
    }
}

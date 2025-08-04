using Core.Commands.Commands;
using MediatR;

namespace Api.Auth;

public class JwtMiddleware(RequestDelegate next, IMediator mediator)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        var attribute = ctx.GetEndpoint()?.Metadata.OfType<RequireAuthAttribute>().FirstOrDefault();

        if (attribute is null)
        {
            await next(ctx);
            return;
        }

        const string tokenType = "Bearer ";

        var headerContent = ctx.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(headerContent) || !headerContent.StartsWith(tokenType))
        {
            await ApiResponse.ApplyAsync(ctx, ApiResponse.Unauthorized());
            return;
        }

        var token = headerContent[tokenType.Length..];

        var result = await mediator.Send(new AuthorizeCommand(token));

        if (result.IsFailure)
        {
            await ApiResponse.ApplyAsync(ctx, ApiResponse.Unauthorized());
            return;
        }

        var optionalActivationAttribute = ctx.GetEndpoint()
            ?.Metadata.OfType<OptionalActivationAttribute>()
            .FirstOrDefault();

        var payload = result.Value;

        if (!payload.IsActivated && optionalActivationAttribute is null)
        {
            await ApiResponse.ApplyAsync(
                ctx,
                ApiResponse.Forbid("Account has not been activated yet")
            );
            return;
        }

        ctx.Items[AuthCtxConstants.AuthUser] = new AuthorizedUser(
            payload.UserId,
            payload.SessionId,
            payload.Role,
            payload.AccessToken,
            payload.LifeTimeLeft
        );

        await next(ctx);
    }
}

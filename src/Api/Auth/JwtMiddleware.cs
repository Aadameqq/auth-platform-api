using Core.Queries;

namespace Api.Auth;

public class JwtMiddleware(RequestDelegate next, GetTokenPayloadQueryHandler tokenQueryHandler)
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

        var result = await tokenQueryHandler.Execute(token);

        if (result.IsFailure)
        {
            await ApiResponse.ApplyAsync(ctx, ApiResponse.Unauthorized());
            return;
        }

        ctx.Items["authorizedUser"] = new AuthorizedUser(
            result.Value.UserId,
            result.Value.SessionId,
            result.Value.Role
        );
        await next(ctx);
    }
}

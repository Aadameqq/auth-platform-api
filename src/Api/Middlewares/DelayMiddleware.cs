namespace Api.Middlewares;

public class DelayMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        if (
            ctx.Request.Query.TryGetValue("delayMs", out var delayStr)
            && int.TryParse(delayStr, out var delayMs)
        )
        {
            await Task.Delay(delayMs);
        }

        await next(ctx);
    }
}

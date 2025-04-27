using Api.Controllers.Dtos;
using Core.Commands.Commands;
using Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OAuthController(IMediator mediator) : ControllerBase
{
    [HttpGet("/o-auth/{provider}/url")]
    public async Task<ActionResult<GetOAuthUrlResponse>> Get(
        [FromQuery] string redirectUri,
        [FromRoute] string provider
    )
    {
        var result = await mediator.Send(new InitializeOAuthCommand(redirectUri, provider));

        if (result is { IsFailure: true, Exception: InvalidOAuthProvider })
        {
            return ApiResponse.NotFound("Invalid oAuth provider");
        }

        return new GetOAuthUrlResponse(result.Value.Url, result.Value.StateId);
    }
}

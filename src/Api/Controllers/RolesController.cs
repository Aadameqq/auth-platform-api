using Api.Auth;
using Api.Controllers.Dtos;
using Core.Domain;
using Core.Queries.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [RequireAuth]
    public async Task<ActionResult<GetAllRolesResponse>> GetAll(
        [FromAuth] AccessManager accessManager
    )
    {
        if (!accessManager.HasAnyRole(Role.Admin))
        {
            return ApiResponse.Forbid();
        }

        var result = await mediator.Send(new ListRolesQuery());

        return new GetAllRolesResponse(result.Value);
    }
}

using System.ComponentModel.DataAnnotations;

namespace Api.Controllers.Dtos;

public record CreateAccountBody(
    [Required] string Username,
    [Required] string Email,
    [Required] [MinLength(12)] string Password
);

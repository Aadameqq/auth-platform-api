namespace Api.Controllers.Dtos;

public record GetOAuthUrlResponse(string Url, Guid StateId);

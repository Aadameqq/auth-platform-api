namespace Api.Controllers.Dtos;

public record OAuthBody(string StateToken, Guid StateId, string Code);

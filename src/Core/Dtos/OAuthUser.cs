namespace Core.Dtos;

public record OAuthUser(string OAuthId, string UserName, string Email, string Provider) { }

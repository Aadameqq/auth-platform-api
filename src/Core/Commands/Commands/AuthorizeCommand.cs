using Core.Dtos;

namespace Core.Commands.Commands;

public record AuthorizeCommand(string AccessToken) : Command<AccessTokenPayload>;

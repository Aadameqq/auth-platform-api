namespace Core.Commands.Outputs;

public record CreatedAccountOutput(string AccessToken, string RefreshToken, Guid ConfirmationId) { }

namespace Api.Controllers.Dtos;

public record TokenPairWithConfirmationResponse(
    string AccessToken,
    string RefreshToken,
    Guid ConfirmationId
);

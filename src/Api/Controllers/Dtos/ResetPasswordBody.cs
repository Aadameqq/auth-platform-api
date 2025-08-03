namespace Api.Controllers.Dtos;

public record ResetPasswordBody(Guid ConfirmationId, string NewPassword);

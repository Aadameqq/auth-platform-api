namespace Api.Controllers.Dtos;

public record ResetPasswordBody(string NewPassword, string Code);

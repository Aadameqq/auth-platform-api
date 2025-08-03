using Core.Domain;

namespace Api.Controllers.Dtos;

public record CreateConfirmationBody(ConfirmableAction Action, ConfirmationMethod Method) { }

using Core.Domain;

namespace Core.Dtos;

public record ConfirmationDto(Guid Id, ConfirmationMethod Method, ConfirmableAction Action) { }

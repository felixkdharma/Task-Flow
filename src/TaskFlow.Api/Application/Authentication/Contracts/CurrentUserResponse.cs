namespace TaskFlow.src.Application.Authentication.Contracts;

public sealed record CurrentUserResponse(Guid Id, string Email, string DisplayName);

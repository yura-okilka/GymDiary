namespace GymDiary.Api.SystemTests.Infrastructure.TestServer.ApiModels;

public record ErrorResponse(string Name, string Message, object? Details);

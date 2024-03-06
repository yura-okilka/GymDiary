namespace GymDiary.Api.SystemTests.Infrastructure.TestServer.ApiClients.Models;

public record ErrorResponse(string Name, string Message, object? Details);

namespace GymDiary.Api.SystemTests.TestServer.ApiClients.Models;

public record ErrorResponse(string Name, string Message, object? Details);

namespace GymDiary.Api.SystemTests.Infrastructure.TestServer.GymDiaryApiModels;

public record ErrorResponse(string Name, string Message, object? Details);

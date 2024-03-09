namespace GymDiary.Api.SystemTests.TestApp.ApiClients.Models;

public static class CreateSportsman
{
    public record Request
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderType? Gender { get; set; }
    }

    public enum GenderType
    {
        Male,
        Female,
        Other
    }

    public record Response(string Id);
}

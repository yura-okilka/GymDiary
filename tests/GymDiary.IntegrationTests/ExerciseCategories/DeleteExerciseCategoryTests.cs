using FluentAssertions;

using GymDiary.ApiClient.Generated.Models;

using Microsoft.AspNetCore.Http;

namespace GymDiary.IntegrationTests.ExerciseCategories;

[Collection(nameof(AppHostCollection))]
public class DeleteExerciseCategoryTests(AppHostFixture fixture)
{
    [Fact(Skip = "Needs the FakeUserContext's user id seeded in the test database before the Create workflow accepts it.")]
    public async Task Deletes_the_category_that_was_just_created()
    {
        var client = fixture.GetApiClient();

        var created = await client.ExerciseCategories.PostAsync(new Request { Name = "Cardio" });

        created.Should().NotBeNull();
        created.Id.Should().NotBeEmpty();

        await client.ExerciseCategories[created.Id!.Value].DeleteAsync();

        var act = () => client.ExerciseCategories[created.Id!.Value].GetAsync();

        var ex = await act.Should().ThrowAsync<ProblemDetails>();
        ex.Which.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Returns_not_found_when_the_category_does_not_exist()
    {
        var client = fixture.GetApiClient();

        var missingId = Guid.NewGuid();

        var act = () => client.ExerciseCategories[missingId].DeleteAsync();

        var ex = await act.Should().ThrowAsync<ProblemDetails>();
        ex.Which.Status.Should().Be(StatusCodes.Status404NotFound);
        ex.Which.Title.Should().Be("Exercise category not found");
        ex.Which.Detail.Should().Be($"Exercise category '{missingId}' was not found for this user.");
    }
}

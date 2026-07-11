using FluentAssertions;
using GymDiary.ApiClient.Generated.Models;
using Microsoft.AspNetCore.Http;
using HttpValidationProblemDetails = GymDiary.ApiClient.Generated.Models.HttpValidationProblemDetails;

namespace GymDiary.IntegrationTests.ExerciseCategories;

[Collection(nameof(AppHostCollection))]
public class RenameExerciseCategoryTests(AppHostFixture fixture)
{
    [Fact(Skip = "Needs the FakeUserContext's user id seeded in the test database before the Create workflow accepts it.")]
    public async Task Renames_the_category_that_was_just_created()
    {
        var client = fixture.GetApiClient();

        var created = await client.ExerciseCategories.PostAsync(new Request { Name = "Cardio" });

        created.Should().NotBeNull();
        created.Id.Should().NotBeEmpty();

        await client.ExerciseCategories[created.Id!.Value].PutAsync(new Request { Name = "Conditioning" });

        var category = await client.ExerciseCategories[created.Id!.Value].GetAsync();

        category.Should().NotBeNull();
        category.Id.Should().Be(created.Id);
        category.Name.Should().Be("Conditioning");
    }

    [Fact]
    public async Task Returns_validation_problem_when_name_is_invalid()
    {
        var client = fixture.GetApiClient();

        var validId = Guid.NewGuid();
        var tooLongName = new string('x', 51);

        var act = () => client.ExerciseCategories[validId].PutAsync(new Request { Name = tooLongName });

        var ex = await act.Should().ThrowAsync<HttpValidationProblemDetails>();
        ex.Which.Status.Should().Be(StatusCodes.Status400BadRequest);
        ex.Which.Errors.Should().NotBeNull();
        ex.Which.Errors!.AdditionalData.Should().ContainKey("Name");
    }

    [Fact]
    public async Task Returns_not_found_when_the_category_does_not_exist()
    {
        var client = fixture.GetApiClient();

        var missingId = Guid.NewGuid();

        var act = () => client.ExerciseCategories[missingId].PutAsync(new Request { Name = "Cardio" });

        var ex = await act.Should().ThrowAsync<ProblemDetails>();
        ex.Which.Status.Should().Be(StatusCodes.Status404NotFound);
        ex.Which.Title.Should().Be("Exercise category not found");
        ex.Which.Detail.Should().Be($"Exercise category '{missingId}' was not found for this user.");
    }
}

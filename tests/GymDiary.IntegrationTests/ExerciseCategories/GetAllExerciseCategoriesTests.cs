using FluentAssertions;

using GymDiary.ApiClient.Generated.Models;

namespace GymDiary.IntegrationTests.ExerciseCategories;

[Collection(nameof(AppHostCollection))]
public class GetAllExerciseCategoriesTests(AppHostFixture fixture)
{
    [Fact]
    public async Task Returns_ok_with_a_collection_for_the_authenticated_user()
    {
        var client = fixture.GetApiClient();

        // The endpoint resolves the owner from the authenticated (fake) user context rather than from
        // user input, so the only reachable outcome through the client is a 200 with a collection — the
        // Kiota client would have thrown on any non-success status. The collection is empty until the
        // user owns categories, but every item it does contain must be a fully populated DTO.
        var categories = await client.ExerciseCategories.GetAsync();

        categories.Should().NotBeNull();
        categories.Should().OnlyContain(c =>
            c.Id.HasValue && c.Id.Value != Guid.Empty &&
            !string.IsNullOrEmpty(c.Name) &&
            c.OwnerId.HasValue && c.OwnerId.Value != Guid.Empty);
    }

    [Fact(Skip = "Needs the FakeUserContext's user id seeded in the test database before the Create workflow accepts it.")]
    public async Task Returns_the_categories_owned_by_the_user()
    {
        var client = fixture.GetApiClient();

        var legs = await client.ExerciseCategories.PostAsync(new Request { Name = "Legs" });
        var chest = await client.ExerciseCategories.PostAsync(new Request { Name = "Chest" });

        legs.Should().NotBeNull();
        chest.Should().NotBeNull();

        var categories = await client.ExerciseCategories.GetAsync();

        categories.Should().NotBeNull();
        categories.Select(c => c.Id).Should().Contain(legs.Id).And.Contain(chest.Id);
        categories.Select(c => c.Name).Should().Contain(["Legs", "Chest"]);
        categories.Select(c => c.OwnerId).Distinct().Should().ContainSingle()
            .Which.Should().NotBeEmpty();
    }
}

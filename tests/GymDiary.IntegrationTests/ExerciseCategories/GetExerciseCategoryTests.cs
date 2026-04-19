using FluentAssertions;

namespace GymDiary.IntegrationTests.ExerciseCategories;

[Collection(nameof(AppHostCollection))]
public class GetExerciseCategoryTests(AppHostFixture fixture)
{
    [Fact]
    public async Task Returns_exercise_category_with_matching_id()
    {
        var client = fixture.GetApiClient();

        var response = await client.ExerciseCategories["abc"].GetAsync();

        response.Should().NotBeNull();
        response!.Id.Should().Be("sample-abc");
    }
}

namespace GymDiary.IntegrationTests;

/// <summary>
///     All integration tests tagged with <c>[Collection(nameof(AppHostCollection))]</c> share a single
///     <see cref="AppHostFixture" /> instance — the AppHost is built, started and disposed exactly once
///     per test-run for this collection.
/// </summary>
[CollectionDefinition(nameof(AppHostCollection))]
public sealed class AppHostCollection : ICollectionFixture<AppHostFixture>
{
}

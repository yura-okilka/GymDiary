using GymDiary.Core.Time;

using Microsoft.Extensions.Time.Testing;

namespace GymDiary.Api.SystemTests.TestServer.Fakes;

/// <summary>
///     A wrapper around <see cref="FakeTimeProvider" /> because it doesn't allow going back in time.
/// </summary>
public class FakeClock : IClock
{
    private FakeTimeProvider _time = null!;

    public FakeClock() => Reset();

    public void Reset() => _time = new FakeTimeProvider(DateTime.UtcNow);

    public DateTime UtcNow => _time.GetUtcNow().UtcDateTime;

    public void SetUtcNow(DateTime value) => _time.SetUtcNow(value);

    public void Advance(TimeSpan delta) => _time.Advance(delta);
}

using GymDiary.Api.SystemTests.TestApp.ApiClients;
using GymDiary.Api.SystemTests.TestApp.Fakes;

namespace GymDiary.Api.SystemTests.TestApp;

public interface IGymDiaryApp
{
    IGymDiaryApiClient Client { get; }
    GymDiaryAppFakes Fakes { get; }
    void Reset();
}

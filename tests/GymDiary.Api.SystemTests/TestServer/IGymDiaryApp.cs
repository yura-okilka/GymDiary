using GymDiary.Api.SystemTests.TestServer.ApiClients;
using GymDiary.Api.SystemTests.TestServer.Fakes;

namespace GymDiary.Api.SystemTests.TestServer;

public interface IGymDiaryApp
{
    IGymDiaryApiClient Client { get; }
    GymDiaryAppFakes Fakes { get; }
    void Reset();
}

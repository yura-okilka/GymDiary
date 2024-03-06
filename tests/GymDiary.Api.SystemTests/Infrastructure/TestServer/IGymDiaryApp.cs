using GymDiary.Api.SystemTests.Infrastructure.TestServer.ApiClients;
using GymDiary.Api.SystemTests.Infrastructure.TestServer.Fakes;

namespace GymDiary.Api.SystemTests.Infrastructure.TestServer;

public interface IGymDiaryApp
{
    IGymDiaryApiClient Client { get; }
    GymDiaryAppFakes Fakes { get; }
    void Reset();
}

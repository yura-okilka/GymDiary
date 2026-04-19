using GymDiary.AppHost;

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var mongoUsername = builder.AddParameter("mongo-username", "admin");
var mongoPassword = builder.AddParameter("mongo-password", "admin", secret: true);

var mongoDb = builder.AddMongoDB(ResourceNames.MongoDb, userName: mongoUsername, password: mongoPassword)
    .WithContainerName("gymdiary-mongodb")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint("tcp",
        ep =>
        {
            // Workaround to expose port https://github.com/dotnet/aspire/issues/6532#issuecomment-2466653817
            ep.Port = 27019;
            ep.IsProxied = false;
        });

var gymDiaryDb = mongoDb.AddDatabase(ResourceNames.GymDiaryDb, "gymDiary");

var api = builder.AddProject<GymDiary_Api>(ResourceNames.Api)
    .WithHttpHealthCheck("/health")
    .WithReference(gymDiaryDb)
    .WaitFor(gymDiaryDb);

builder.Build().Run();

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var mongoUsername = builder.AddParameter("mongo-username", "admin");
var mongoPassword = builder.AddParameter("mongo-password", "admin", secret: true);

var mongoDb = builder.AddMongoDB("mongo-db", userName: mongoUsername, password: mongoPassword)
    .WithContainerName("gymdiary-mongodb")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint("tcp",
        ep =>
        {
            // Workaround to expose port https://github.com/dotnet/aspire/issues/6532#issuecomment-2466653817
            ep.Port = 27019;
            ep.IsProxied = false;
        });

var gymDiaryDb = mongoDb.AddDatabase("gymdiary-db", "gymDiary");

var api = builder.AddProject<GymDiary_Api_CSharp>("api")
    .WithHttpHealthCheck("/health")
    .WithReference(gymDiaryDb)
    .WaitFor(gymDiaryDb);

builder.Build().Run();

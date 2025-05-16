using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<GymDiary_Api_CSharp>("api")
    .WithHttpsHealthCheck("/health");

builder.Build().Run();

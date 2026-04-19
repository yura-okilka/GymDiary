using GymDiary.Api.Authentication;
using GymDiary.Api.Endpoints;
using GymDiary.Api.OpenApi;
using GymDiary.Application;
using GymDiary.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi(options => options.AddOperationTransformer<ProblemResponseTransformer>());
builder.Services.AddProblemDetails();
builder.Services.AddGymDiaryEndpoints();
builder.Services.AddGymDiaryAuthentication();
builder.Services.AddGymDiaryApplication();
builder.Services.AddGymDiaryInfrastructure();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();
app.MapEndpoints();

app.Run();

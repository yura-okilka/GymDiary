using System.Text.Json.Serialization;
using GymDiary.Api.Authentication;
using GymDiary.Api.Endpoints;
using GymDiary.Api.OpenApi;
using GymDiary.Api.Time;
using GymDiary.Application;
using GymDiary.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Strict number handling so the OpenAPI schema emits `status` as just `integer` instead of `["integer","string"]`.
// See https://github.com/dotnet/aspnetcore/issues/64501 — Kiota falls back to UntypedNode for the union type.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict);

builder.Services.AddOpenApi(options => options.AddOperationTransformer<ProblemResponseTransformer>());
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddGymDiaryTime();
builder.Services.AddGymDiaryEndpoints();
builder.Services.AddGymDiaryAuthentication();
builder.Services.AddGymDiaryApplication();
builder.Services.AddGymDiaryInfrastructure();

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();
app.MapEndpoints();

app.Run();

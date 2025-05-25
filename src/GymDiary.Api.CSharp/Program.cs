using GymDiary.Api.CSharp;
using GymDiary.Api.CSharp.Endpoints;
using GymDiary.Application;
using GymDiary.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddEndpoints(AssemblyReference.Assembly);
builder.Services.AddGymDiaryApplication();
builder.Services.AddGymDiaryInfrastructure();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.MapDefaultEndpoints();
app.MapEndpoints();

app.Run();

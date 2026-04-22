using DotNetEnv;
using Technical_Challenge.WebAPI.Extensions;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
var workerMode = args.Contains("--worker") || builder.Configuration.GetValue<bool>("WORKER_MODE");

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("development.settings.json", optional: true, reloadOnChange: true);
}

builder.Services
    .AddPersistenceServices(builder.Configuration)
    .AddScraperServices();

if (workerMode)
{
    builder.Services.AddWorkerHostedService();
}
else
{
    builder.Services.AddWebApiServices(builder.Configuration);
}

var app = builder.Build();

await app.InitializeDatabaseAsync();

if (!workerMode)
{
    app.ConfigureWebApi();
}

app.Run();

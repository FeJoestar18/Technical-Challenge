using DotNetEnv;
using Technical_Challenge.WebAPI.Extensions;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("development.settings.json", optional: true, reloadOnChange: true);
}

builder.Services
    .AddWebApiServices(builder.Configuration)
    .AddPersistenceServices(builder.Configuration)
    .AddScraperServices();

var app = builder.Build();

await app.InitializeDatabaseAsync();
app.ConfigureWebApi();

app.Run();

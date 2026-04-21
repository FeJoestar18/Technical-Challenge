using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Technical_Challenge.Application;
using Technical_Challenge.Application.Interfaces;
using Technical_Challenge.Infrastructure.Persistence.Context;
using Technical_Challenge.Infrastructure.Repositories;
using Technical_Challenge.Infrastructure.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("development.settings.json", optional: true, reloadOnChange: true);
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Currency Quotes API", Version = "v1" });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();

builder.Services.AddHttpClient<IScraperService, HttpScraperService>();
builder.Services.AddScoped<IScraperService, HttpScraperService>();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var efContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await efContext.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Quotes API v1"));

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();

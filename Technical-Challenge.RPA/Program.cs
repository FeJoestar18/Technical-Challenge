using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Technical_Challenge.Application;
using Technical_Challenge.Application.Interfaces;
using Technical_Challenge.Infrastructure.Persistence.Context;
using Technical_Challenge.Infrastructure.Repositories;
using Technical_Challenge.Infrastructure.Services;

Env.Load();

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

builder.Services.AddHttpClient<IScraperService, HttpScraperService>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

await app.RunAsync();

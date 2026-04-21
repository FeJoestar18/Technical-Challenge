using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Technical_Challenge.Application;
using Technical_Challenge.Application.Interfaces;
using Technical_Challenge.Domain.Entities;
using Xunit;

namespace Technical_Challenge.Test;

public class WorkerTests
{
    private static IConfiguration BuildConfig(int intervalMinutes = 1)
    {
        var dict = new Dictionary<string, string?>
        {
            ["Scraper:IntervalMinutes"] = intervalMinutes.ToString()
        };
        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    [Fact]
    public async Task Worker_CallsFetchAndSave_WhenExecuted()
    {
        var scraperMock = new Mock<IScraperService>();
        var repoMock = new Mock<IQuoteRepository>();

        var quotes = new List<CurrencyQuote>
        {
            new() { Currency = "USD-BRL", Bid = 5.10m, Ask = 5.12m, High = 5.15m, Low = 5.05m, CapturedAt = DateTime.UtcNow }
        };

        scraperMock.Setup(s => s.FetchQuotesAsync()).ReturnsAsync(quotes);
        repoMock.Setup(r => r.SaveAsync(quotes)).Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddScoped<IScraperService>(_ => scraperMock.Object);
        services.AddScoped<IQuoteRepository>(_ => repoMock.Object);
        var provider = services.BuildServiceProvider();

        var worker = new Worker(
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<Worker>.Instance,
            BuildConfig(intervalMinutes: 60) 
        );

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        await worker.StartAsync(cts.Token);
        await Task.Delay(500); 
        await worker.StopAsync(CancellationToken.None);

        scraperMock.Verify(s => s.FetchQuotesAsync(), Times.AtLeastOnce);
        repoMock.Verify(r => r.SaveAsync(It.IsAny<List<CurrencyQuote>>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Worker_DoesNotThrow_WhenScraperFails()
    {
        var scraperMock = new Mock<IScraperService>();
        var repoMock = new Mock<IQuoteRepository>();

        scraperMock.Setup(s => s.FetchQuotesAsync())
                   .ThrowsAsync(new HttpRequestException("timeout"));

        var services = new ServiceCollection();
        services.AddScoped<IScraperService>(_ => scraperMock.Object);
        services.AddScoped<IQuoteRepository>(_ => repoMock.Object);
        var provider = services.BuildServiceProvider();

        var worker = new Worker(
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<Worker>.Instance,
            BuildConfig(intervalMinutes: 60)
        );

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        await worker.StartAsync(cts.Token);
        await Task.Delay(500);
        await worker.StopAsync(CancellationToken.None);

        repoMock.Verify(r => r.SaveAsync(It.IsAny<List<CurrencyQuote>>()), Times.Never);
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Technical_Challenge.Application.Interfaces;

namespace Technical_Challenge.Application;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<Worker> _logger;
    private readonly int _intervalMinutes;

    public Worker(IServiceScopeFactory scopeFactory, ILogger<Worker> logger, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _intervalMinutes = configuration.GetValue<int>("Scraper:IntervalMinutes", 5);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Currency RPA Worker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var scraper = scope.ServiceProvider.GetRequiredService<IScraperService>();
                
                var repository = scope.ServiceProvider.GetRequiredService<IQuoteRepository>();

                _logger.LogInformation("Buscando cotações em {Time}", DateTimeOffset.UtcNow);

                var quotes = await scraper.FetchQuotesAsync();
                await repository.SaveAsync(quotes);
                
                _logger.LogInformation("{Count} cotação(ões) salvas.", quotes.Count);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro HTTP ao buscar cotações.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no Worker.");
            }

            await Task.Delay(TimeSpan.FromMinutes(_intervalMinutes), stoppingToken);
        }
    }
}

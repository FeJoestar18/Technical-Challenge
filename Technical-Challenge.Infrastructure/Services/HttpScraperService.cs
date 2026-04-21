using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Technical_Challenge.Application.Interfaces;
using Technical_Challenge.Domain.Entities;

namespace Technical_Challenge.Infrastructure.Services;

public class HttpScraperService : IScraperService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpScraperService> _logger;
    private readonly string _targetUrl;

    public HttpScraperService(HttpClient httpClient, ILogger<HttpScraperService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _targetUrl = configuration["Scraper:TargetUrl"]
            ?? "https://economia.awesomeapi.com.br/json/last/USD-BRL,EUR-BRL,BTC-BRL,EUR-USD";
    }

    public async Task<List<CurrencyQuote>> FetchQuotesAsync()
    {
        _logger.LogDebug("GET {Url}", _targetUrl);
        
        var httpResponse = await _httpClient.GetAsync(_targetUrl);
        httpResponse.EnsureSuccessStatusCode();

        var response = await httpResponse.Content.ReadAsStringAsync();

        var quotes = new List<CurrencyQuote>();
        using var doc = JsonDocument.Parse(response);

        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            var el = prop.Value;
            var code = el.GetProperty("code").GetString() ?? string.Empty;
            var codeIn = el.GetProperty("codein").GetString() ?? string.Empty;

            quotes.Add(new CurrencyQuote
            {
                Currency = $"{code}-{codeIn}",
                Bid = ParseDecimal(el, "bid"),
                Ask = ParseDecimal(el, "ask"),
                High = ParseDecimal(el, "high"),
                Low = ParseDecimal(el, "low"),
                CapturedAt = DateTime.UtcNow
            });
        }

        return quotes;
    }

    private static decimal ParseDecimal(JsonElement el, string property)
    {
        var raw = el.GetProperty(property).GetString() ?? "0";
        return decimal.Parse(raw, CultureInfo.InvariantCulture);
    }
}

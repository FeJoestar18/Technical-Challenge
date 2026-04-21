using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using Technical_Challenge.Infrastructure.Services;
using Xunit;

namespace Technical_Challenge.Test;

public class HttpScraperServiceTests
{
    private static IConfiguration BuildConfig(string url = "https://fake-api/json/last/USD-BRL")
    {
        var dict = new Dictionary<string, string?>
        {
            ["Scraper:TargetUrl"] = url
        };

        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    private static HttpClient BuildHttpClient(string json)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        return new HttpClient(handlerMock.Object);
    }

    [Fact]
    public async Task FetchQuotesAsync_ReturnsParsedQuotes()
    {
        const string json = """
            {
              "USDBRL": {
                "code": "USD",
                "codein": "BRL",
                "bid": "5.1050",
                "ask": "5.1100",
                "high": "5.1500",
                "low": "5.0900"
              },
              "EURBRL": {
                "code": "EUR",
                "codein": "BRL",
                "bid": "5.5200",
                "ask": "5.5300",
                "high": "5.5800",
                "low": "5.4900"
              }
            }
            """;

        var service = new HttpScraperService(
            BuildHttpClient(json),
            NullLogger<HttpScraperService>.Instance,
            BuildConfig());

        var quotes = await service.FetchQuotesAsync();

        Assert.Equal(2, quotes.Count);

        var usd = quotes.First(q => q.Currency == "USD-BRL");
        Assert.Equal(5.1050m, usd.Bid);
        Assert.Equal(5.1100m, usd.Ask);
        Assert.Equal(5.1500m, usd.High);
        Assert.Equal(5.0900m, usd.Low);

        var eur = quotes.First(q => q.Currency == "EUR-BRL");
        Assert.Equal(5.5200m, eur.Bid);
    }

    [Fact]
    public async Task FetchQuotesAsync_ThrowsHttpRequestException_WhenServerFails()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var httpClient = new HttpClient(handlerMock.Object);
        
        var service = new HttpScraperService(
            httpClient,
            NullLogger<HttpScraperService>.Instance,
            BuildConfig());

        await Assert.ThrowsAsync<HttpRequestException>(() => service.FetchQuotesAsync());
    }
}

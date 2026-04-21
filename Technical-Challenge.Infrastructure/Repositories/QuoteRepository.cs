using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Technical_Challenge.Application.Interfaces;
using Technical_Challenge.Domain.Entities;

namespace Technical_Challenge.Infrastructure.Repositories;

public class QuoteRepository : IQuoteRepository
{
    private readonly string _connectionString;

    public QuoteRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection not configured.");
    }

    private NpgsqlConnection CreateConnection() => new(_connectionString);

    public async Task SaveAsync(List<CurrencyQuote> quotes)
    {
        using var conn = CreateConnection();
        const string sql = """
            INSERT INTO quotes (currency, bid, ask, high, low, captured_at)
            VALUES (@Currency, @Bid, @Ask, @High, @Low, @CapturedAt)
            """;
        await conn.ExecuteAsync(sql, quotes);
    }

    public async Task<IEnumerable<CurrencyQuote>> GetAllAsync(int page, int pageSize)
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT id AS "Id", currency AS "Currency", bid AS "Bid", ask AS "Ask",
                   high AS "High", low AS "Low", captured_at AS "CapturedAt"
            FROM quotes
            ORDER BY captured_at DESC
            LIMIT @PageSize OFFSET @Offset
            """;
        return await conn.QueryAsync<CurrencyQuote>(sql, new
        {
            PageSize = pageSize,
            Offset = (page - 1) * pageSize
        });
    }

    public async Task<IEnumerable<CurrencyQuote>> GetLatestAsync()
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT DISTINCT ON (currency)
                   id AS "Id", currency AS "Currency", bid AS "Bid", ask AS "Ask",
                   high AS "High", low AS "Low", captured_at AS "CapturedAt"
            FROM quotes
            ORDER BY currency, captured_at DESC
            """;
        return await conn.QueryAsync<CurrencyQuote>(sql);
    }

    public async Task<IEnumerable<CurrencyQuote>> GetByCurrencyAsync(string currency)
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT id AS "Id", currency AS "Currency", bid AS "Bid", ask AS "Ask",
                   high AS "High", low AS "Low", captured_at AS "CapturedAt"
            FROM quotes
            WHERE UPPER(currency) = UPPER(@Currency)
            ORDER BY captured_at DESC
            """;
        return await conn.QueryAsync<CurrencyQuote>(sql, new { Currency = currency });
    }

    public async Task<IEnumerable<CurrencyQuote>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT id AS "Id", currency AS "Currency", bid AS "Bid", ask AS "Ask",
                   high AS "High", low AS "Low", captured_at AS "CapturedAt"
            FROM quotes
            WHERE captured_at BETWEEN @From AND @To
            ORDER BY captured_at DESC
            """;
        return await conn.QueryAsync<CurrencyQuote>(sql, new { From = from, To = to });
    }
}

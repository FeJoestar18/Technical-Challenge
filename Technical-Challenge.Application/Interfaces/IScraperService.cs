using Technical_Challenge.Domain.Entities;

namespace Technical_Challenge.Application.Interfaces;

public interface IScraperService
{
    Task<List<CurrencyQuote>> FetchQuotesAsync();
}

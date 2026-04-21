using Technical_Challenge.Domain.Entities;

namespace Technical_Challenge.Application.Interfaces;

public interface IQuoteRepository
{
    Task SaveAsync(List<CurrencyQuote> quotes);
    Task<IEnumerable<CurrencyQuote>> GetAllAsync(int page, int pageSize);
    Task<IEnumerable<CurrencyQuote>> GetLatestAsync();
    Task<IEnumerable<CurrencyQuote>> GetByCurrencyAsync(string currency);
    Task<IEnumerable<CurrencyQuote>> GetByDateRangeAsync(DateTime from, DateTime to);
}

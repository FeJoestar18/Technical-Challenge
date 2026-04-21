namespace Technical_Challenge.Domain.Entities;

public class CurrencyQuote
{
    public int Id { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public DateTime CapturedAt { get; set; }
}

using Microsoft.AspNetCore.Mvc;
using Technical_Challenge.Application.Interfaces;
using Technical_Challenge.Domain.Entities;

namespace Technical_Challenge.WebAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/quotes")]
[Route("api/quotes")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteRepository _repository;

    public QuotesController(IQuoteRepository repository)
    {
        _repository = repository;
    }

    /// <summary>Lista todas as cotações com paginação.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CurrencyQuote>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 100) pageSize = 20;

        var result = await _repository.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>Última cotação de cada moeda.</summary>
    [HttpGet("latest")]
    public async Task<ActionResult<IEnumerable<CurrencyQuote>>> GetLatest()
    {
        var result = await _repository.GetLatestAsync();
        return Ok(result);
    }

    /// <summary>Histórico de uma moeda específica (ex: USD-BRL).</summary>
    [HttpGet("{currency}")]
    public async Task<ActionResult<IEnumerable<CurrencyQuote>>> GetByCurrency(string currency)
    {
        var result = await _repository.GetByCurrencyAsync(currency);
        return Ok(result);
    }

    /// <summary>Cotações em um intervalo de datas.</summary>
    [HttpGet("range")]
    public async Task<ActionResult<IEnumerable<CurrencyQuote>>> GetByDateRange(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        if (from > to)
            return BadRequest("O parâmetro 'from' deve ser anterior ao 'to'.");

        var result = await _repository.GetByDateRangeAsync(from, to);
        return Ok(result);
    }
}

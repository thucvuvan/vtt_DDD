using Application.Abstractions;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace OrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventItemsController : ControllerBase
{
    private readonly IEventItemListService _eventItems;

    public EventItemsController(IEventItemListService eventItems)
    {
        _eventItems = eventItems;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EventItemListItem>), StatusCodes.Status200OK)]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await _eventItems.GetAllAsync(cancellationToken);
        return Ok(items);
    }
}

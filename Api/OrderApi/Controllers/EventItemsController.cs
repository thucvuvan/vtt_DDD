using Application.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace OrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventItemsController : ControllerBase
{
    private readonly IEventItemReadRepository _eventItems;

    public EventItemsController(IEventItemReadRepository eventItems)
    {
        _eventItems = eventItems;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EventItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await _eventItems.GetAllAsync(cancellationToken);
        return Ok(items);
    }
}

using Application.Dtos;
using Domain.Entities;

namespace Application.Mappings;

public static class EventItemListItemMapping
{
    public static EventItemListItem ToDto(this EventItem e) => new()
    {
        Id = e.Id,
        EvItTitle = e.EvItTitle,
        EvItSubtitle = e.EvItSubtitle,
        EvItDescription = e.EvItDescription,
        EvItInitialStock = e.EvItInitialStock,
        EvItAvailableStock = e.EvItAvailableStock,
        EvItIsStockPrepared = e.EvItIsStockPrepared,
        EvItOriginalPrice = e.EvItOriginalPrice,
        EvItFlashPrice = e.EvItFlashPrice,
        EvItStartTime = e.EvItStartTime,
        EvItEndTime = e.EvItEndTime,
        EvItRules = e.EvItRules,
        EvItStatus = e.EvItStatus,
        EvItActivityId = e.EvItActivityId,
        EvItUpdatedAt = e.EvItUpdatedAt,
        EvItCreatedAt = e.EvItCreatedAt,
    };
}

using Application.Dtos;

namespace Application.Abstractions;

public interface IEventItemListService
{
    Task<IReadOnlyList<EventItemListItem>?> GetAllAsync(CancellationToken cancellationToken = default);
}

using Domain.Entities;

namespace Application.Abstractions;

public interface IEventItemReadRepository
{
    Task<IReadOnlyList<EventItem>> GetAllAsync(CancellationToken cancellationToken = default);
}

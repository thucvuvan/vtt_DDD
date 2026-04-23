using Application.Abstractions;
using Application.Dtos;
using Application.Mappings;

namespace Application.Services;

public sealed class EventItemListService : IEventItemListService
{
    private readonly IEventItemReadRepository _repository;

    public EventItemListService(IEventItemReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<EventItemListItem>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(e => e.ToDto()).ToList();
    }
}

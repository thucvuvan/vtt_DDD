using System.Text.Json;
using Application.Abstractions;
using Application.Dtos;
using Application.Mappings;

namespace Application.Services;

public sealed class EventItemListService : IEventItemListService
{
    private const string EventItemsCacheKey = "vtt:orderapi:eventitems:all";
    private static readonly TimeSpan EventItemsCacheTtl = TimeSpan.FromMinutes(1);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly IEventItemReadRepository _repository;
    private readonly ICacheService _cache;

    public EventItemListService(IEventItemReadRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IReadOnlyList<EventItemListItem>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetStringAsync(EventItemsCacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
            var fromCache = JsonSerializer.Deserialize<List<EventItemListItem>>(cached, JsonOptions);
            if (fromCache is not null)
                return fromCache;
        }

        var entities = await _repository.GetAllAsync(cancellationToken);
        var list = entities.Select(e => e.ToDto()).ToList();
        await _cache.SetStringAsync(
            EventItemsCacheKey,
            JsonSerializer.Serialize(list, JsonOptions),
            EventItemsCacheTtl,
            cancellationToken);
        return list;
    }
}

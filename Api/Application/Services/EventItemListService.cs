using System.Text.Json;
using Application.Abstractions;
using Application.Dtos;
using Application.Mappings;

namespace Application.Services;

public sealed class EventItemListService : IEventItemListService
{
    private const string EventItemsCacheKey = "vtt:orderapi:eventitems:all";
    private const string EventItemsCacheLockKey = "lock:vtt:orderapi:eventitems:all";
    private static readonly TimeSpan EventItemsCacheTtl = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan CacheLockTtl = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan LockRetryDelay = TimeSpan.FromMilliseconds(80);

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
        var cachedItems = await TryGetFromCacheAsync(cancellationToken);
        if (cachedItems is not null)
            return cachedItems;

        var lockValue = Guid.NewGuid().ToString("N");
        var lockAcquired = await _cache.TryAcquireLockAsync(
            EventItemsCacheLockKey,
            lockValue,
            CacheLockTtl,
            cancellationToken);

        if (lockAcquired)
            return await LoadAndCacheUnderLockAsync(lockValue, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            //Console.WriteLine("Request LOCK, poll cache");
            await Task.Delay(LockRetryDelay, cancellationToken);

            cachedItems = await TryGetFromCacheAsync(cancellationToken);
            if (cachedItems is not null)
                return cachedItems;

            lockAcquired = await _cache.TryAcquireLockAsync(
                EventItemsCacheLockKey,
                lockValue,
                CacheLockTtl,
                cancellationToken);

            if (lockAcquired)
                return await LoadAndCacheUnderLockAsync(lockValue, cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return Array.Empty<EventItemListItem>();
    }

    private async Task<IReadOnlyList<EventItemListItem>?> TryGetFromCacheAsync(CancellationToken cancellationToken)
    {
        var cached = await _cache.GetStringAsync(EventItemsCacheKey, cancellationToken);
        if (string.IsNullOrEmpty(cached))
            return null;

        return JsonSerializer.Deserialize<List<EventItemListItem>>(cached, JsonOptions);
    }

    private async Task<IReadOnlyList<EventItemListItem>> LoadAndCacheUnderLockAsync(
        string lockValue,
        CancellationToken cancellationToken)
    {
        try
        {
            var cachedItems = await TryGetFromCacheAsync(cancellationToken);
            if (cachedItems is not null)
            {
                //Console.WriteLine("LoadAndCacheUnderLockAsync -> TryGetFromCacheAsync");
                return cachedItems;
            }

            Console.WriteLine("LoadFromDatabaseAndCacheAsync");
            return await LoadFromDatabaseAndCacheAsync(cancellationToken);
        }
        finally
        {
            await _cache.ReleaseLockAsync(EventItemsCacheLockKey, lockValue, cancellationToken);
        }
    }

    private async Task<IReadOnlyList<EventItemListItem>> LoadFromDatabaseAndCacheAsync(
        CancellationToken cancellationToken)
    {
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

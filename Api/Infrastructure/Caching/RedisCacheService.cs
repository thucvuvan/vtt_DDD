using Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Caching;

public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default) =>
        await _cache.GetStringAsync(key, cancellationToken);

    public async Task SetStringAsync(
        string key,
        string value,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();
        if (absoluteExpirationRelativeToNow.HasValue)
            options.SetAbsoluteExpiration(absoluteExpirationRelativeToNow.Value);
        await _cache.SetStringAsync(key, value, options, token: cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        await _cache.RemoveAsync(key, cancellationToken);
}

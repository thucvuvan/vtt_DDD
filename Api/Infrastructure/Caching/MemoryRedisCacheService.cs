using Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace Infrastructure.Caching;

public sealed class MemoryRedisCacheService : ICacheService
{
    private const string ReleaseLockScript = "if redis.call('get', KEYS[1]) == ARGV[1] then return redis.call('del', KEYS[1]) else return 0 end";

    private readonly IDistributedCache _cache;
    private readonly IMemoryCache _memoryCache;
    private readonly IDatabase _redisDb;

    public MemoryRedisCacheService(
        IDistributedCache cache,
        IMemoryCache memoryCache,
        IConnectionMultiplexer multiplexer)
    {
        _cache = cache;
        _memoryCache = memoryCache;
        _redisDb = multiplexer.GetDatabase();
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default) =>
        await _cache.GetStringAsync(key, cancellationToken);

    public async Task<string?> GetMemOrRedis(string key, CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue<string>(key, out var cachedInMemory))
            return cachedInMemory;

        var cachedInRedis = await _cache.GetStringAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(cachedInRedis))
            _memoryCache.Set(key, cachedInRedis);

        return cachedInRedis;
    }

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

    public async Task SetMemAndRedis(
        string key,
        string value,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        CancellationToken cancellationToken = default)
    {
        if (absoluteExpirationRelativeToNow.HasValue)
            _memoryCache.Set(key, value, absoluteExpirationRelativeToNow.Value);
        else
            _memoryCache.Set(key, value);

        var options = new DistributedCacheEntryOptions();
        if (absoluteExpirationRelativeToNow.HasValue)
            options.SetAbsoluteExpiration(absoluteExpirationRelativeToNow.Value);
        await _cache.SetStringAsync(key, value, options, token: cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        await RemoveFromMemAndRedisAsync(key, cancellationToken);

    private async Task RemoveFromMemAndRedisAsync(string key, CancellationToken cancellationToken)
    {
        _memoryCache.Remove(key);
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> TryAcquireLockAsync(
        string key,
        string value,
        TimeSpan expiry,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _redisDb.StringSetAsync(key, value, expiry, when: When.NotExists);
    }

    public async Task<bool> ReleaseLockAsync(
        string key,
        string value,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = (long)await _redisDb.ScriptEvaluateAsync(
            ReleaseLockScript,
            keys: new RedisKey[] { key },
            values: new RedisValue[] { value });
        return result > 0;
    }
}

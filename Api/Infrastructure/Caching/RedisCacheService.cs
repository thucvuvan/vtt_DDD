using Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Infrastructure.Caching;

public sealed class RedisCacheService : ICacheService
{
    private const string ReleaseLockScript = "if redis.call('get', KEYS[1]) == ARGV[1] then return redis.call('del', KEYS[1]) else return 0 end";

    private readonly IDistributedCache _cache;
    private readonly IDatabase _redisDb;

    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer multiplexer)
    {
        _cache = cache;
        _redisDb = multiplexer.GetDatabase();
    }

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

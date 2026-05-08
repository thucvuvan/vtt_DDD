using Application.Abstractions;

namespace Infrastructure.Caching;

/// <summary>Không lưu cache; dùng khi không cấu hình Redis.</summary>
public sealed class NoOpCacheService : ICacheService
{
    public Task<string?> GetRedisStringAsync(string key, CancellationToken cancellationToken = default) =>
        Task.FromResult<string?>(null);

    public Task<string?> GetMemOrRedis(string key, CancellationToken cancellationToken = default) =>
        Task.FromResult<string?>(null);

    public Task SetStringAsync(
        string key,
        string value,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task SetMemAndRedis(
        string key,
        string value,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<bool> TryAcquireLockAsync(
        string key,
        string value,
        TimeSpan expiry,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(true);

    public Task<bool> ReleaseLockAsync(
        string key,
        string value,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(true);

    public string? GetMemoryString(string key, CancellationToken cancellationToken = default)
    {
        return null;
    }

    public Task SetMemoryString(string key, string value, TimeSpan? absoluteExpirationRelativeToNow = null, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

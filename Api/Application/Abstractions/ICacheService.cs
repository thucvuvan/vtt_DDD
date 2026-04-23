namespace Application.Abstractions;

/// <summary>Trừu tượng bộ đệm phân tán; triển khai nằm ở Infrastructure (Redis, v.v.).</summary>
public interface ICacheService
{
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);
    Task SetStringAsync(
        string key,
        string value,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}

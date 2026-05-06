using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace OnlineCourses.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _defaultSlidingExpiration = TimeSpan.FromMinutes(2);

    public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<T?> GetAsync<T>(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("GetAsync called with null or empty key");
                return Task.FromResult(default(T));
            }

            if (_cache.TryGetValue(key, out T? value))
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return Task.FromResult(value);
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            return Task.FromResult(default(T));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting value from cache for key: {Key}", key);
            return Task.FromResult(default(T));
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("SetAsync called with null or empty key");
                return Task.CompletedTask;
            }

            if (value == null)
            {
                _logger.LogWarning("SetAsync called with null value for key: {Key}", key);
                return Task.CompletedTask;
            }

            var expirationTime = expiration ?? _defaultExpiration;
            
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime,
                SlidingExpiration = expirationTime > _defaultSlidingExpiration ? _defaultSlidingExpiration : null,
                Priority = CacheItemPriority.Normal
            };

            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _logger.LogDebug("Cache entry evicted. Key: {Key}, Reason: {Reason}", key, reason);
            });

            _cache.Set(key, value, options);
            _logger.LogDebug("Cache set successfully for key: {Key}, Expiration: {Expiration}", key, expirationTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while setting value in cache for key: {Key}", key);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("RemoveAsync called with null or empty key");
                return Task.CompletedTask;
            }

            _cache.Remove(key);
            _logger.LogDebug("Cache entry removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing cache entry for key: {Key}", key);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("ExistsAsync called with null or empty key");
                return Task.FromResult(false);
            }

            var exists = _cache.TryGetValue(key, out _);
            _logger.LogDebug("Cache exists check for key: {Key}, Result: {Exists}", key, exists);
            return Task.FromResult(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking cache existence for key: {Key}", key);
            return Task.FromResult(false);
        }
    }
}
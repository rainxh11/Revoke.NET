namespace Revoke.NET;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Internals;

public class MemoryBlackList : IBlackList
{
    private readonly ConcurrentDictionary<string, DateTime> _blackList;
    private readonly TimeSpan? _defaultTtl;

    private MemoryBlackList(TimeSpan? defaultTtl)
    {
        this._defaultTtl = defaultTtl;
        this._blackList = new ConcurrentDictionary<string, DateTime>();
    }

    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> DeleteAsync(string string, CancellationToken cancellationToken = default) instead.",
        false)]
    public Task<bool> Delete(string key)
    {
        Ensure.Is.NotNullOrWhiteSpace(key, nameof(key));

        return Task.FromResult(this._blackList.TryRemove(key, out _));
    }

    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> RevokeAsync(string key, TimeSpan expireAfter, CancellationToken cancellationToken = default) instead.",
        false)]
    public Task<bool> Revoke(
        string key,
        TimeSpan expireAfter)
    {
        Ensure.Is.NotNullOrWhiteSpace(key, nameof(key));

        return Task.FromResult(this._blackList.TryAdd(key, DateTime.Now.Add(expireAfter)));
    }

    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> RevokeAsync(string key, DateTime expireOn, CancellationToken cancellationToken = default) instead.",
        false)]
    public Task<bool> Revoke(
        string key,
        DateTime expireOn)
    {
        Ensure.Is.NotNullOrWhiteSpace(key, nameof(key));

        if (expireOn < DateTimeOffset.Now)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(this._blackList.TryAdd(key, expireOn));
    }

    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> DeleteAllAsync(CancellationToken cancellationToken = default) instead.",
        false)]
    public Task DeleteAll()
    {
        this._blackList.Clear();

        return Task.CompletedTask;
    }

    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> IsRevokedAsync(string key, CancellationToken cancellationToken = default) instead.",
        false)]
    public Task<bool> IsRevoked(string key)
    {
        Ensure.Is.NotNullOrWhiteSpace(key, nameof(key));
        if (this._blackList.TryGetValue(key, out DateTime item))
        {
            return Task.FromResult(item >= DateTime.Now);
        }

        return Task.FromResult(false);
    }

    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> RevokeAsync(string key, CancellationToken cancellationToken = default) instead.",
        false)]
    public Task<bool> Revoke(string key)
    {
        Ensure.Is.NotNullOrWhiteSpace(key, nameof(key));

        if (DateTime.Now.Add(this._defaultTtl ?? TimeSpan.MaxValue) < DateTime.Now)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(this._blackList.TryAdd(key, DateTime.Now.Add(this._defaultTtl ?? TimeSpan.MaxValue)));
    }

    public Task<bool> DeleteAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Ensure.Is.NotNullOrWhiteSpace(key, nameof(key));

        return Task.FromResult(this._blackList.TryRemove(key, out _));
    }

    public Task<bool> RevokeAsync(
        string key,
        TimeSpan expireAfter,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Ensure.Is.NotNullOrWhiteSpace(key, nameof(key));

        return Task.FromResult(this._blackList.TryAdd(key, DateTime.Now.Add(expireAfter)));
    }

    public Task<bool> RevokeAsync(
        string key,
        DateTime expireOn,
        CancellationToken cancellationToken = default)

    {
        cancellationToken.ThrowIfCancellationRequested();

        Ensure.Is.NotNullOrWhiteSpace(key, nameof(key));

        if (expireOn < DateTimeOffset.Now)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(this._blackList.TryAdd(key, expireOn));
    }

    public Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        this._blackList.Clear();

        return Task.CompletedTask;
    }

    public Task<bool> IsRevokedAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Ensure.Is.NotNullOrWhiteSpace(key, nameof(key));

        if (this._blackList.TryGetValue(key, out DateTime item))
        {
            return Task.FromResult(item >= DateTime.Now);
        }

        return Task.FromResult(false);
    }

    public Task<bool> RevokeAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Ensure.Is.NotNullOrWhiteSpace(key, nameof(key));

        if (DateTime.Now.Add(this._defaultTtl ?? TimeSpan.MaxValue) < DateTime.Now)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(this._blackList.TryAdd(key, DateTime.Now.Add(this._defaultTtl ?? TimeSpan.MaxValue)));
    }

    public static MemoryBlackList CreateStore(TimeSpan? defaultExpirationDuration = null)
    {
        return new MemoryBlackList(defaultExpirationDuration);
    }
}
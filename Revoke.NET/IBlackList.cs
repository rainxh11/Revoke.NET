namespace Revoke.NET;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

public interface IBlackList
{
    Task<bool> RevokeAsync(string key, TimeSpan expireAfter, CancellationToken cancellationToken = default);
    
    
    
    
    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> RevokeAsync(string key, TimeSpan expireAfter, CancellationToken cancellationToken  = default) instead.",
        false)]
    Task<bool> Revoke(
        string key,
        TimeSpan expireAfter);
    

    Task<bool> RevokeAsync(
        string key,
        DateTime expireOn, CancellationToken cancellationToken  = default);
    
    
    
    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> RevokeAsync(string key, CancellationToken cancellationToken  = default) instead.",
        false)]
    Task<bool> Revoke(
        string key,
        DateTime expireOn);

    Task<bool> RevokeAsync(string key, CancellationToken cancellationToken  = default);
    
    
    
    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> RevokeAsync(string key, CancellationToken cancellationToken  = default) instead.",
        false)]
    Task<bool> Revoke(string key);
    

    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken  = default);
    
    
    
    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> DeleteAsync(string key, CancellationToken cancellationToken  = default)instead.",
        false)]
    Task<bool> Delete(string key);
    

    Task DeleteAllAsync(CancellationToken cancellationToken = default);
    
    
    
    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> DeleteAllAsync(CancellationToken cancellationToken = default) instead.",
        false)]
    Task DeleteAll();
    

    Task<bool> IsRevokedAsync(string key, CancellationToken cancellationToken  = default);
    
    
    
    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >>  IsRevokedAsync(string key, CancellationToken cancellationToken  = default) instead.",
        false)]
    Task<bool> IsRevoked(string key);
}
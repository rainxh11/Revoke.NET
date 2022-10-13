namespace Revoke.NET;

using System;
using System.Threading.Tasks;

public interface IBlackList
{
    Task<bool> Revoke(string key, TimeSpan expireAfter);

    Task<bool> Revoke(string key, DateTime expireOn);

    Task<bool> Revoke(string key);

    Task<bool> Delete(string key);

    Task DeleteAll();

    Task<bool> IsRevoked(string key);
}
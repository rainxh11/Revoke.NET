using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Revoke.NET
{
    public interface IBlackListStore
    {
        Task<IEnumerable<T>> GetAll<T>() where T : IBlackListItem;
        Task<T> Get<T>(string key) where T : IBlackListItem;
        Task<bool> Delete(string key);
        Task<bool> Revoke(string key);
        Task<bool> Revoke(string key, TimeSpan expireAfter);
        Task<bool> Revoke(string key, DateTimeOffset expireOn);
        Task<bool> Revoke<T>(T item) where T : IBlackListItem;
        Task DeleteExpired();
        Task DeleteAll();
        Task<bool> IsRevoked(string key);
    }
}
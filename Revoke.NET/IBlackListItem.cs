using System;

namespace Revoke.NET
{
    public class BlackListItem : IBlackListItem
    {
        public BlackListItem(string key, DateTimeOffset expireOn)
        {
            this.Key = key;
            this.ExpireOn = expireOn;
        }

        public BlackListItem(string key, TimeSpan expireAfter)
        {
            this.Key = key;
            this.ExpireOn = DateTimeOffset.Now.Add(expireAfter);
        }

        public string Key { get; }
        public DateTimeOffset ExpireOn { get; }
    }

    public interface IBlackListItem
    {
        string Key { get; }
        DateTimeOffset ExpireOn { get; }
    }
}
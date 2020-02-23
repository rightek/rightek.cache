using System;

using Rightek.Cache.Enums;

namespace Rightek.Cache
{
    public interface IConfig
    {
        IConfig WithTimeToLive(long ttl);

        IConfig WithTimeUnit(TimeUnit unit);

        IConfig WithExpirationType(ExpirationType expType);

        IMemoryCache WithKey(string key);

        IMemoryCache Configure(Action<Settings> configure);

        void SetDefault(Action<Settings> configure);
    }
}
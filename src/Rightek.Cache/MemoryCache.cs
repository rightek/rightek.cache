using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

using Rightek.Cache.Enums;

namespace Rightek.Cache
{
    public class MemoryCache : IMemoryCache, IConfig
    {
        readonly System.Runtime.Caching.MemoryCache _cache;
        readonly Settings _defaultSettings;

        Settings _settings;

        #region Singleton

        static readonly Lazy<MemoryCache> _lazy = new Lazy<MemoryCache>(() => new MemoryCache());
        public static IConfig Instance => _lazy.Value;

        MemoryCache()
        {
            _cache = System.Runtime.Caching.MemoryCache.Default;
            _defaultSettings = new Settings();
        }

        #endregion Singleton

        Settings _currentSetting
        {
            get
            {
                var s = new Settings
                {
                    Key = _settings?.Key ?? _defaultSettings.Key,
                    ExpirationType = _settings?.ExpirationType ?? _defaultSettings.ExpirationType,
                    TimeToLive = _settings?.TimeToLive ?? _defaultSettings.TimeToLive
                };

                _settings = null;

                return s;
            }
        }

        public IConfig WithTimeToLive(long ttl)
        {
            if (ttl < 0) throw new ArgumentException("TimeToLive must be greater than zero");

            if (_settings == null) _settings = new Settings();

            _settings.TimeToLive = ttl;

            return this;
        }

        public IConfig WithTimeUnit(TimeUnit unit)
        {
            if (_settings == null) _settings = new Settings();

            _settings.TimeUnit = unit;

            return this;
        }

        public IConfig WithExpirationType(ExpirationType expType)
        {
            if (_settings == null) _settings = new Settings();

            _settings.ExpirationType = expType;

            return this;
        }

        public IMemoryCache WithKey(string key)
        {
            key.ThrowIfNullOrEmpty(nameof(key));

            if (_settings == null) _settings = new Settings();

            _settings.Key = key;

            return this;
        }

        public IMemoryCache Configure(Action<Settings> configure)
        {
            _settings = new Settings();

            configure(_settings);

            return this;
        }

        public void SetDefault(Action<Settings> configure)
        {
            configure(_defaultSettings);
        }

        public T Get<T>() => AddOrGetExisting<T>(() => default);

        public T AddOrGetExisting<T>(Func<T> valueFactory)
        {
            valueFactory.ThrowIfNull(nameof(valueFactory));

            var settings = _currentSetting;

            settings.Key.ThrowIfNullOrEmpty("key");

            var newValue = new Lazy<T>(valueFactory);
            var oldValue = _cache.AddOrGetExisting(settings.Key, newValue, getPolicy(settings)) as Lazy<T>;

            try
            {
                return (oldValue ?? newValue).Value;
            }
            catch
            {
                _cache.Remove(settings.Key);
                throw;
            }
        }

        public async Task<T> AddOrGetExistingAsync<T>(Func<Task<T>> valueFactory)
        {
            valueFactory.ThrowIfNull(nameof(valueFactory));

            var settings = _currentSetting;

            settings.Key.ThrowIfNullOrEmpty("key");

            var newValue = new AsyncLazy<T>(valueFactory);
            var oldValue = _cache.AddOrGetExisting(settings.Key, newValue, getPolicy(settings)) as Lazy<T>;

            try
            {
                return oldValue != null ? oldValue.Value : await newValue.Value;
            }
            catch
            {
                _cache.Remove(settings.Key);
                throw;
            }
        }

        public void Set<T>(T value) => AddOrGetExisting<T>(() => value);

        public void Remove()
        {
            var settings = _currentSetting;

            settings.Key.ThrowIfNullOrEmpty("key");

            _cache.Remove(settings.Key);
        }

        public bool Exists()
        {
            var settings = _currentSetting;

            settings.Key.ThrowIfNullOrEmpty("key");

            return _cache.Contains(settings.Key);
        }

        CacheItemPolicy getPolicy(Settings settings)
        {
            var policy = new CacheItemPolicy();

            if (settings.TimeToLive > 0)
            {
                if (settings.ExpirationType == ExpirationType.ABSOLUTE)
                {
                    switch (settings.TimeUnit)
                    {
                        case TimeUnit.MILLISECOND:
                            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(settings.TimeToLive);
                            break;

                        case TimeUnit.SECOND:
                            policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(settings.TimeToLive);
                            break;

                        case TimeUnit.MINUTE:
                            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(settings.TimeToLive);
                            break;

                        case TimeUnit.HOUR:
                            policy.AbsoluteExpiration = DateTimeOffset.Now.AddHours(settings.TimeToLive);
                            break;

                        case TimeUnit.DAY:
                            policy.AbsoluteExpiration = DateTimeOffset.Now.AddDays(settings.TimeToLive);
                            break;
                    }
                }
                else if (settings.ExpirationType == ExpirationType.SLIDING)
                {
                    switch (settings.TimeUnit)
                    {
                        case TimeUnit.MILLISECOND:
                            policy.SlidingExpiration = TimeSpan.FromMilliseconds(settings.TimeToLive);
                            break;

                        case TimeUnit.SECOND:
                            policy.SlidingExpiration = TimeSpan.FromSeconds(settings.TimeToLive);
                            break;

                        case TimeUnit.MINUTE:
                            policy.SlidingExpiration = TimeSpan.FromMinutes(settings.TimeToLive);
                            break;

                        case TimeUnit.HOUR:
                            policy.SlidingExpiration = TimeSpan.FromHours(settings.TimeToLive);
                            break;

                        case TimeUnit.DAY:
                            policy.SlidingExpiration = TimeSpan.FromDays(settings.TimeToLive);
                            break;
                    }
                }
            }
            else policy.Priority = CacheItemPriority.NotRemovable;

            return policy;
        }
    }

    static class TypeLock<T>
    {
        public static object Lock { get; } = new object();
    }
}
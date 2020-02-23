# Rightek.Cache
Simple wrapper around `System.Runtime.Caching`

## Basic Usage

```cs
var key = Guid.NewGuid().ToString();
var expected = "value";

// Add value to cache
MemoryCache.Instance.WithKey(key).Set(expected);

// Get value from cache
var actual = MemoryCache.Instance.WithKey(key).Get<string>();
```

## Nuget [![nuget](https://img.shields.io/nuget/v/Rightek.Cache.svg?color=%23268bd2&style=flat-square)](https://www.nuget.org/packages/Rightek.Cache) [![stats](https://img.shields.io/nuget/dt/Rightek.Cache.svg?color=%2382b414&style=flat-square)](https://www.nuget.org/stats/packages/Rightek.Cache?groupby=Version)

`PM> Install-Package Rightek.Cache`

## Config API

- `WithTimeToLive`: Specify how long value should be stay in cache
- `WithTimeUnit`: `TimeToLive` unit, available values
	- MILLISECOND _defalut_
	- SECOND
	- MINUTE
	- HOUR
	- DAY
- `WithExpirationType`: Expiration policy, available values
	- ABSOLUTE _defalut_ _[MSDN](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.caching.cacheitempolicy.slidingexpiration)_
	- SLIDING _[MSDN](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.caching.cacheitempolicy.absoluteexpiration)_
- `WithKey`: Cache item key, it should be unique per item
- `Configure`: You can use this method instead of all above methods and set all configs at once
- `SetDefault`: You should call it once at application startup, default setting can be Override using above methods

## Core API

- `Set`: Add item to cache
- `Get`: Get item from cache
- `AddOrGetExisting`: Get item from cache or add it to the cache and then return it
- `AddOrGetExistingAsync`: Same as `AddOrGetExisting` but `async`
- `Remove`: Remove item from cache
- `Exists`: Check to see item with specific key is exists in cache

## License
MIT

---
Made with ♥ by people @ [Rightek](http://rightek.ir)

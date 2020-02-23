using System;

using FluentAssertions;

using Xunit;

namespace Rightek.Cache.Test
{
    public class MemoryCacheTests
    {
        [Fact]
        public void ValueBeforeSetWithoutAnyExtraSettingsShouldBeNull()
        {
            var key = Guid.NewGuid().ToString();

            var actual = MemoryCache.Instance.WithKey(key).Get<string>();

            actual.Should().Be(default);
        }

        [Fact]
        public void ValueAfterSetWithoutAnyExtraSettingsShouldReturns()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "value";

            MemoryCache.Instance.WithKey(key).Set(expected);

            var actual = MemoryCache.Instance.WithKey(key).Get<string>();

            actual.Should().Be(expected);
        }

        [Fact]
        public void ValueWithoutAnyExtraSettingsShouldStayInCache()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "value";

            MemoryCache.Instance.WithKey(key).Set(expected);

            System.Threading.Thread.Sleep(1000);

            var actual = MemoryCache.Instance.WithKey(key).Get<string>();

            actual.Should().Be(expected);
        }

        [Fact]
        public void CacheShouldExpireAfterPassTimeToLive()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "value";

            MemoryCache.Instance.WithTimeToLive(1000).WithKey(key).Set(expected);

            System.Threading.Thread.Sleep(1001);

            var actual = MemoryCache.Instance.WithKey(key).Get<string>();

            actual.Should().Be(default);
        }

        [Fact]
        public void CallingRemoveBeforeSetShouldProduceNoError()
        {
            var key = Guid.NewGuid().ToString();

            MemoryCache.Instance.WithKey(key).Remove();
        }

        [Fact]
        public void CacheShouldBeExistsAfterSet()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "value";

            MemoryCache.Instance.WithKey(key).Set(expected);

            MemoryCache.Instance.WithKey(key).Exists().Should().BeTrue();
        }

        [Fact]
        public void RemoveAfterSetShouldWork()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "value";

            MemoryCache.Instance.WithKey(key).Set(expected);

            MemoryCache.Instance.WithKey(key).Remove();

            var actual = MemoryCache.Instance.WithKey(key).Get<string>();

            actual.Should().Be(default);
        }

        [Fact]
        public void CacheShouldBeSameAsResultOfValueFactoryAndThisValueShouldBeAddedToCache()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "value";

            var actual = MemoryCache.Instance.WithKey(key).AddOrGetExisting(() => expected);
            actual.Should().Be(expected);

            actual = MemoryCache.Instance.WithKey(key).Get<string>();
            actual.Should().Be(expected);
        }
    }
}
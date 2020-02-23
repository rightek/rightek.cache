using System;
using System.Threading.Tasks;

namespace Rightek.Cache
{
    public interface IMemoryCache
    {
        void Set<T>(T value);

        T Get<T>();

        T AddOrGetExisting<T>(Func<T> factory);

        Task<T> AddOrGetExistingAsync<T>(Func<Task<T>> factory);

        void Remove();

        bool Exists();
    }
}
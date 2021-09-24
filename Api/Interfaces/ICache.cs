using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace TheSwamp.Api.Interfaces
{
    public interface ICache
    {
        Task<TItem> GetOrCreateAsync<TItem>(object key, Func<ICacheEntry, Task<TItem>> factory);
    }
}

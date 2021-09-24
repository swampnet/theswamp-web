using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Api.Interfaces;

namespace TheSwamp.Api.Services
{
    class Cache : ICache
    {
        private readonly MemoryCache _cache;

        public Cache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions()
            {
                ExpirationScanFrequency = TimeSpan.FromMinutes(1)
            });
        }


        public Task<TItem> GetOrCreateAsync<TItem>(object key, Func<ICacheEntry, Task<TItem>> factory)
        {
            return _cache.GetOrCreateAsync(key, factory);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Api.DAL;
using TheSwamp.Api.DAL.API;
using TheSwamp.Api.Interfaces;

namespace TheSwamp.Api.Services
{
    internal class Auth : IAuth
    {
        private readonly ICache _cache;
        private readonly ApiContext _context;

        public Auth(ICache cache, ApiContext context)
        {
            _cache = cache;
            _context = context;
        }


        public async Task<bool> AuthenticateAsync(HttpRequest req)
        {
            if (req.Headers.TryGetValue("X-api-key", out var apiKeys))
            {
                var key = apiKeys.First();

                var keys = await _cache.GetOrCreateAsync("api-key", LoadApiKeys);

                return keys.Contains(key);
            }

            return false;
        }


        private async Task<string[]> LoadApiKeys(ICacheEntry arg)
        {
            arg.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            return await _context.Keys
                .Where(k => k.IsEnabled)
                .Select(k => k.Key)
                .ToArrayAsync();
        }
    }
}

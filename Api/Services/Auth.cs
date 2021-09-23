using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheSwamp.Api.Interfaces;

namespace TheSwamp.Api.Services
{
    internal class Auth : IAuth
    {
        public Auth()
        {

        }

        public bool Authenticate(HttpRequest req)
        {
            if (req.Headers.TryGetValue("X-api-key", out var apiKeys))
            {
                var key = apiKeys.First();

                // @TODO: Hit cache / database for api keys
                return string.CompareOrdinal(key, "my-super-secret-key") == 0;
            }

            return false;
        }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TheSwamp.Api.Interfaces
{

    public interface IAuth
    {
        Task<bool> AuthenticateAsync(HttpRequest req);
    }
}

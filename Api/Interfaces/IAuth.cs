using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Api.Interfaces
{
    public interface IAuth
    {
        bool Authenticate(HttpRequest req);
    }
}

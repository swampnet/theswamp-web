using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace TheSwamp.Api
{
    static class HttpRequestExtensions
    {
        // https://stackoverflow.com/questions/37582553/how-to-get-client-ip-address-in-azure-functions-c
        public static string GetClientIp(this HttpRequest request)
        {
            IPAddress result = null;
            if (request.Headers.TryGetValue("X-Forwarded-For", out StringValues values))
            {
                var ipn = values.FirstOrDefault().Split(new char[] { ',' }).FirstOrDefault().Split(new char[] { ':' }).FirstOrDefault();
                IPAddress.TryParse(ipn, out result);
            }
            if (result == null)
            {
                result = request.HttpContext.Connection.RemoteIpAddress;
            }

            return result?.ToString();
        }

        public static string GetClientIp(this HttpRequestMessage request)
        {
            IEnumerable<string> values;
            if (request.Headers.TryGetValues("X-Forwarded-For", out values))
            {
                return values.FirstOrDefault().Split(new char[] { ',' }).FirstOrDefault().Split(new char[] { ':' }).FirstOrDefault();
            }

            return "";
        }
    }
}

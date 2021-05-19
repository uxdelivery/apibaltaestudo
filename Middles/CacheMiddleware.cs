using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace APICrudBasica.Middles{

    public class CacheMiddleware {

       private readonly RequestDelegate next;
        public CacheMiddleware(RequestDelegate _next) => next = _next;

        public async Task Invoke(HttpContext ctx){

            ctx.Request.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(3600)

                    };

            await next(ctx);
        }
    }
}
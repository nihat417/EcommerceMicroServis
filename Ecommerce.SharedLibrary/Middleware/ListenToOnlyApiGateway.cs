using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            var signedHeader = httpContext.Request.Headers["Api-Gateway"];

            if(signedHeader.FirstOrDefault() != null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await httpContext.Response.WriteAsync("Sorry,servcie is unavaible");
                return;
            }
            else
            {
                await next(httpContext);
            }
        }
    }
}

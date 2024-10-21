using Ecommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Ecommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            string message = "Sorry Internal server error";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(httpContext);

                if(httpContext.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many request made";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(httpContext, title, message,statusCode);
                }

                if(httpContext.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "tou authorized not succses";
                    await ModifyHeader(httpContext, title, message, statusCode);
                }

                if (httpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "out of access";
                    message = "you are not allowed/required to access";
                    statusCode = StatusCodes.Status403Forbidden;
                    await ModifyHeader(httpContext, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                LogException.LogExtensions(ex);
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "out of time";
                    message = "Request timeout ... try again";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }

                await ModifyHeader(httpContext, title, message, statusCode);
            }
        }

        private static async Task ModifyHeader(HttpContext httpContext, string title, string message, int statusCode)
        {
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Title = title,
                Status = statusCode
            }),CancellationToken.None);
        }

        
    }
}

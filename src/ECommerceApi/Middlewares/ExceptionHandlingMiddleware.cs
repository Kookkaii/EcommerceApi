using System.Net;
using System.Text.Json;

namespace ECommerceApi.Middlewares
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message) : base(message) { }
    }
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidRequestException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            var response = new
            {
                status = (int)statusCode,
                error = statusCode.ToString(),
                message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
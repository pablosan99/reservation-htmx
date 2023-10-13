using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using Reservation.Backend;
using Reservation.Frontend.Resources;

namespace Reservation.Frontend.Pages;

public class BusinessExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public BusinessExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        { 
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 400;
            var errorMessage = "Ups coś poszło nie tak";
            var _message = ERRORS.ResourceManager.GetString(ex.Error);
            if (_message is not null)
            {
                errorMessage = _message;
            }
            var obj = new { exception = errorMessage };
            context.Response.Headers.Add(Htmx.HtmxResponseHeaders.Keys.Trigger, JsonSerializer.Serialize(obj));
            // await context.Response.WriteAsync(); //WriteAsJsonAsync(new { code = ex.Error });
        }
        catch(Exception ex) 
        {
            await HandleExceptionAsync(HttpStatusCode.InternalServerError, context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpStatusCode statusCode, HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) statusCode;

        return context.Response.WriteAsync(exception.Message);
    }
}
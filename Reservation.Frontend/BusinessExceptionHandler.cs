using System.Net;
using System.Text.Json;
using Htmx;
using Reservation.Backend;

namespace Reservation.Frontend.Pages;

public class BusinessExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ErrorProvider _errorProvider;

    public BusinessExceptionMiddleware(RequestDelegate next, ErrorProvider errorProvider)
    {
        _next = next;
        _errorProvider = errorProvider;
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
            var message = _errorProvider.GetMessage(ex);
            context.Response.Headers.Add(HtmxResponseHeaders.Keys.Trigger, JsonSerializer.Serialize(new { exception = message}));
        }
        catch (Exception _)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.BadRequest;

            var message = _errorProvider.GetDefaultMessage();            
            context.Response.Headers.Add(HtmxResponseHeaders.Keys.Trigger, JsonSerializer.Serialize(new { exception = message}));
        }
    }
}
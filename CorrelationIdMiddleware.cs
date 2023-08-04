using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Threading.Tasks;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the correlation ID is already set in the request headers
        if (!context.Request.Headers.ContainsKey("Correlation-Id"))
        {
            // If not, generate a new correlation ID and add it to the response headers
            var correlationId = Guid.NewGuid().ToString();
            context.Response.Headers.Add("Correlation-Id", correlationId);
            context.Request.Headers.Add("Correlation-Id", correlationId);
            MappedDiagnosticsLogicalContext.Set("CorrelationID", correlationId);
            
        }

        // Continue the request pipeline
        await _next(context);
    }
}

using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
// using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        // builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CrudApi2", Version = "v1" });
            });

        builder.Services.AddLogging((lb) =>
        {
            // lb.AddConsole();
            lb.ClearProviders(); // Clear other logging providers
            lb.SetMinimumLevel(LogLevel.Trace); // Set minimum log level
            lb.AddNLog("nlog.config"); //configure with nlog.config
        });

        // Register the correlation ID generator
        // builder.Services.AddScoped<CorrelationIdMiddleware>(); //this is wrong as RequestDelegrate is not possible to inject via DI
        // builder.Host.UseNLog();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        // if (app.Environment.IsDevelopment())
        // {
        app.UseSwagger();
        // app.UseSwaggerUI();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CrudApi2");
        });
        // }

        // app.UseHttpsRedirection();

        app.UseMiddleware<CorrelationIdMiddleware>(); // Use correlation ID middleware


        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
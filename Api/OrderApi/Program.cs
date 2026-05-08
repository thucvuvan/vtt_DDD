using Application;
using Application.Abstractions;
using Application.Services;
using Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Logging.SetMinimumLevel(LogLevel.Critical);
//builder.Logging.AddFilter("Microsoft", LogLevel.Critical);
//builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Critical);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 35000;
        opt.Window = TimeSpan.FromSeconds(1);
        opt.QueueLimit = 0;
    });
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests");
    };
});

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

app.MapPrometheusScrapingEndpoint();

using (var scope = app.Services.CreateScope())
{
    var eventItemListService = scope.ServiceProvider.GetRequiredService<IEventItemListService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("StartupWarmup");
    try
    {
        await eventItemListService.GetAllAsync();
        logger.LogCritical("Warmup completed: EventItemListService.GetAllAsync");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Warmup failed: EventItemListService.GetAllAsync");
    }
}

app.Run();

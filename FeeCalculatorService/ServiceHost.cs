using Microsoft.Extensions.Options;
using Scc.FeeCalculatorService.AppServices;
using Scc.FeeCalculatorService.Configuration;

namespace Scc.FeeCalculatorsService;

public class ServiceHost
{
    public static async Task Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(static builder =>
        {
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("Scc.FeeCalculator.ServiceHost", LogLevel.Debug)
                .AddConsole();
        });

        ILogger logger = loggerFactory.CreateLogger<ServiceHost>();
        logger.LogInformation($"FeeCalculator Service Started: {DateTime.Now}");

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddLogging(builder => builder.AddConsole());

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton((sp) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            FeeOptions options = new();
            config.GetSection("FeeOptions").Bind(options);

            return Options.Create(options);
        });

        builder.Services.AddScoped<IFeeCalculatorAppService, FeeCalculatorAppService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            // Add this
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "API v1");
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        app.MapGet("/health", () => """
            {"status":"okay"}
        """);

        await app.RunAsync();

        logger.LogInformation($"FeeCalculator Service Ended: {DateTime.Now}");
    }
}

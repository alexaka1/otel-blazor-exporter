using OpenTelemetry;
using OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;
using Serilog;
using Serilog.Debugging;

namespace OtelBlazorExporter.Infrastructure.Logging;

public static class Logging
{
    public static void InitStartupLogger()
    {
        // Startup logger
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("SourceContext", "Startup")
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();
    }

    public static void AddLogging(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            SelfLog.Enable(Console.Error);
        }


        builder.Services.AddSerilog((sp, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(sp.GetRequiredService<IConfiguration>())
                    ;
                loggerConfiguration.WriteTo.Async(c => c.OpenTelemetry(openTelemetrySinkOptions =>
                {
                    openTelemetrySinkOptions.ResourceAttributes =
                        new Dictionary<string, object>(
                            sp.GetRequiredService<ResourceCollection>().Detect().Attributes
                        );
                    openTelemetrySinkOptions.OnBeginSuppressInstrumentation = SuppressInstrumentationScope.Begin;
                }));
            },
            writeToProviders: false);
    }
}

using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;

public static class OpenTelemetry
{
    public static IServiceCollection SetupOpenTelemetry(this IServiceCollection services)
    {
        Instrumentation.ActivitySource = new ActivitySource(Instrumentation.ClientServiceName);
        services.AddSingleton<IResourceDetector, ClientSideResource>();
        services.AddSingleton<ResourceCollection>();
        services.AddMetrics();
        services.AddSingleton<OtelBlazorMeterProvider>();
        services.AddSingleton<LoginMetrics>();

        services.AddSingleton<TracerProvider>(sp => Sdk.CreateTracerProviderBuilder()
            .ConfigureResource(r => r.AddDetector(sp.GetRequiredService<ResourceCollection>()))
            .AddSource(Instrumentation.ActivitySource.Name)
            // .AddConsoleExporter()
            .Build()
        );
        services.AddSingleton<MeterProvider>(sp => Sdk.CreateMeterProviderBuilder()
            .ConfigureResource(r => r.AddDetector(sp.GetRequiredService<ResourceCollection>()))
            .AddMeter(Instrumentation.ActivitySource.Name)
            .AddRuntimeInstrumentation()
            .AddMeter(sp.GetRequiredService<OtelBlazorMeterProvider>().Meter.Name)
            .Build()
        );

        return services;
    }
}

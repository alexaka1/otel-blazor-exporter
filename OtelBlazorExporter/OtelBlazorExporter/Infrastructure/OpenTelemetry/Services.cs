using System.Diagnostics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;

namespace OtelBlazorExporter.Infrastructure.OpenTelemetry;

public static class Services
{
    public static IServiceCollection SetupOpenTelemetry(this IServiceCollection services)
    {
        Instrumentation.ActivitySource = new ActivitySource(Instrumentation.ServerServiceName);
        services.AddSingleton<IResourceDetector, ServerSideResource>();
        services.AddSingleton<ResourceCollection>();
        services.AddSingleton<OtelBlazorMeterProvider>();
        services.AddSingleton<LoginMetrics>();

        var openTelemetryBuilder = services.AddOpenTelemetry()
            .ConfigureResource(r => r
                // get all resources in one step
                .AddDetector(s => s.GetRequiredService<ResourceCollection>())
            )
            .WithMetrics(metrics =>
            {
                metrics
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddMeter(OtelBlazorMeterProvider.DefaultMeterName);
                metrics.AddOtlpExporter();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddSource(Instrumentation.ActivitySource.Name);
                tracing.AddOtlpExporter();
            });

        return services;
    }
}

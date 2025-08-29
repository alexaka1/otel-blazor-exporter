using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.JSInterop;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OtelBlazorExporter.Client.Infrastructure.OpenTelemetry.JsInterop;
using OpenTelemetryExporter = OpenTelemetry.Exporter;

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


    [SupportedOSPlatform("browser")]
    public static TracerProviderBuilder AddJsInteropExporter(this TracerProviderBuilder builder,
        IServiceProvider serviceProvider)
    {
        builder.AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Protocol = OpenTelemetryExporter.OtlpExportProtocol.HttpProtobuf;
            otlpOptions.ExportProcessorType = ExportProcessorType.Simple;
            otlpOptions.HttpClientFactory = () =>
            {
                return new HttpClient(
                    new JsInteropMessageHandler(
                        serviceProvider.GetRequiredService<ILogger<JsInteropMessageHandler>>()
                    ), false) { BaseAddress = new Uri("http://localhost") };
            };
        });
        return builder;
    }
}

using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging.Abstractions;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OtelBlazorExporter.Client.Infrastructure.OpenTelemetry.JsInterop;
using OpenTelemetry.Exporter;

namespace OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;

public static class OpenTelemetry
{
    [SupportedOSPlatform("browser")]
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
            .AddJsInteropExporter(sp)
            // .AddConsoleExporter()
            .Build()
        );
        services.AddSingleton<MeterProvider>(sp => Sdk.CreateMeterProviderBuilder()
            .ConfigureResource(r => r.AddDetector(sp.GetRequiredService<ResourceCollection>()))
            .AddMeter(Instrumentation.ActivitySource.Name)
            .AddRuntimeInstrumentation()
            .AddMeter(sp.GetRequiredService<OtelBlazorMeterProvider>().Meter.Name)
            // .AddOtlpExporter(otlpOptions =>
            // {
            //                  otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
            //     otlpOptions.ExportProcessorType = ExportProcessorType.Simple;
            //     otlpOptions.HttpClientFactory = () => new HttpClient(
            //         new JsInteropMessageHandler(
            //             sp.GetRequiredService<ILogger<JsInteropMessageHandler>>()
            //         ), false) { BaseAddress = new Uri("http://localhost") };
            // })
            .Build()
        );

        // Logging via OpenTelemetry (logs) using JS interop exporter
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddOpenTelemetry(o =>
            {
                o.IncludeScopes = true;
                o.ParseStateValues = true;
                o.IncludeFormattedMessage = true;
                // for whatever reason, using the ServiceProvider override duplicates every log message 🫨
                // o.AddProcessor(sp =>
                // {
                //     var logger = sp.GetRequiredService<ILogger<JsInteropMessageHandler>>();
                //     var logExporterOptions = new OtlpExporterOptions
                //     {
                //         Protocol = OtlpExportProtocol.HttpProtobuf,
                //         HttpClientFactory = () => new HttpClient(
                //             new JsInteropMessageHandler(
                //                 logger
                //             ), false) { BaseAddress = new Uri("http://localhost") },
                //         ExportProcessorType = ExportProcessorType.Simple,
                //     };
                //     var logExporter = new OtlpLogExporter(logExporterOptions);
                //     return new SimpleLogRecordExportProcessor(logExporter);
                // });
                var logExporterOptions = new OtlpExporterOptions
                {
                    Protocol = OtlpExportProtocol.HttpProtobuf,
                    HttpClientFactory = () => new HttpClient(
                        new JsInteropMessageHandler(
                            // we don't have DI here and it kinda doesn't make sense to log in a log exporter
                            NullLogger<JsInteropMessageHandler>.Instance
                        ), false) { BaseAddress = new Uri("http://localhost") },
                    ExportProcessorType = ExportProcessorType.Simple,
                };
                var logExporter = new OtlpLogExporter(logExporterOptions);
                o.AddProcessor(new SimpleLogRecordExportProcessor(logExporter));
                o.SetResourceBuilder(ResourceBuilder.CreateEmpty()
                    .AddDetector(sp => sp.GetRequiredService<ResourceCollection>())
                );
            });
        });

        return services;
    }


    [SupportedOSPlatform("browser")]
    public static TracerProviderBuilder AddJsInteropExporter(this TracerProviderBuilder builder,
        IServiceProvider serviceProvider)
    {
        builder.AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
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

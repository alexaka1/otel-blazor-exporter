using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OtelBlazorExporter.Client.Infrastructure;
using OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Logging.ClearProviders();
if (OperatingSystem.IsBrowser())
{
    builder.Services.SetupOpenTelemetry();
    builder.AddLogging();
}

if (OperatingSystem.IsBrowser())
{
    await JSHost.ImportAsync("OtelBlazorExporter.Client.OtlpExport", "../js/otlp-export.js");
}

var host = builder.Build();
var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger($"{nameof(OtelBlazorExporter)}.{nameof(OtelBlazorExporter.Client)}");
_ = host.Services.GetRequiredService<TracerProvider>();
_ = host.Services.GetRequiredService<MeterProvider>();
logger.LogInformation("Starting in WebAssembly mode");

await host
    .RunAsync();

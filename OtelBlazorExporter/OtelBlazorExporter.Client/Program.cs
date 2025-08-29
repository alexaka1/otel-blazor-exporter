using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OtelBlazorExporter.Client.Infrastructure;
using OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;
using System.Runtime.InteropServices.JavaScript;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Logging.ClearProviders();
builder.Services.SetupOpenTelemetry();
builder.AddLogging();
if (OperatingSystem.IsBrowser())
{
    await JSHost.ImportAsync("OtelBlazorExporter.Client.OtlpExport", "/js/otlp-export.js");
}

await builder.Build().RunAsync();

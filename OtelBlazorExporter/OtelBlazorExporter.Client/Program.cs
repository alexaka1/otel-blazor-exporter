using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OtelBlazorExporter.Client.Infrastructure;
using OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Logging.ClearProviders();
builder.Services.SetupOpenTelemetry();
builder.AddLogging();
await builder.Build().RunAsync();

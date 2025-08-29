using System.Diagnostics.Metrics;

namespace OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;

// https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/metrics/README.md
// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics
public class OtelBlazorMeterProvider(IMeterFactory meterFactory)
{
    public readonly Meter Meter = meterFactory.Create(DefaultMeterName);
    public const string DefaultMeterName = "otel.test";
}

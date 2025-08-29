using System.Diagnostics;

namespace OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;

public static class Instrumentation
{
    /// <summary>
    ///     This is the activity source that will be used for all telemetry. It is initialized in the
    ///     services.SetupOpenTelemetry() method.
    /// </summary>
    /// <exception cref="InvalidOperationException">When it has not been initialized.</exception>
    public static ActivitySource ActivitySource
    {
        get => s_activitySource ?? throw new InvalidOperationException("ActivitySource not initialized");
        set
        {
            s_activitySource ??= value;
        }
    }

    public const string ServerServiceName = "otel.test.backend";
    public const string ClientServiceName = "otel.test.frontend";
    public const string Namespace = "otel.test";
    private static ActivitySource? s_activitySource;
}

using System.Runtime.InteropServices;
using OpenTelemetry.Resources;
using OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;

namespace OtelBlazorExporter.Infrastructure.OpenTelemetry;

public class ServerSideResource(IHostEnvironment environment)
    : IResourceDetector
{
    // https://opentelemetry.io/docs/specs/otel/semantic-conventions/
    public Resource Detect()
    {
        var resources = ResourceBuilder.CreateDefault()
                .AddService(Instrumentation.ServerServiceName, Instrumentation.Namespace)
                .AddEnvironmentVariableDetector()
                .AddAttributes([
                    new KeyValuePair<string, object>("dotnet.version", RuntimeInformation.FrameworkDescription),
                    new KeyValuePair<string, object>("dotnet.rid", RuntimeInformation.RuntimeIdentifier),
                    new KeyValuePair<string, object>("deployment.environment.name", environment.EnvironmentName),
                ])
            ;
        return resources.Build();
    }
}

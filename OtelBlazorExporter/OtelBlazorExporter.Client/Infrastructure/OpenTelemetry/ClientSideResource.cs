using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenTelemetry.Resources;

namespace OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;

public class ClientSideResource(IWebAssemblyHostEnvironment environment)
    : IResourceDetector
{
    // https://opentelemetry.io/docs/specs/otel/semantic-conventions/
    public Resource Detect()
    {
        var resources = ResourceBuilder.CreateDefault()
            .AddService(Instrumentation.ClientServiceName, Instrumentation.Namespace)
            .AddEnvironmentVariableDetector()
            .AddAttributes([
                new KeyValuePair<string, object>("dotnet.version", RuntimeInformation.FrameworkDescription),
                new KeyValuePair<string, object>("dotnet.rid", RuntimeInformation.RuntimeIdentifier),
                new KeyValuePair<string, object>("deployment.environment.name", environment.Environment),
            ])
            .Build();
        return resources;
    }
}

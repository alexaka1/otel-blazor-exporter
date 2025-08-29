using OpenTelemetry.Resources;

namespace OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;

public class ResourceCollection(IEnumerable<IResourceDetector> resources) : IResourceDetector
{
    public Resource Detect()
    {
        var builder = ResourceBuilder.CreateEmpty();
        foreach (var detector in resources)
        {
            builder.AddDetector(detector);
        }

        return builder.Build();
    }
}

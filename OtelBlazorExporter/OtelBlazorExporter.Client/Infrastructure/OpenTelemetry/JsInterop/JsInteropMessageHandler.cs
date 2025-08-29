using System.Net;
using System.Runtime.Versioning;
using Microsoft.JSInterop;
using OpenTelemetry;

namespace OtelBlazorExporter.Client.Infrastructure.OpenTelemetry.JsInterop;

[SupportedOSPlatform("browser")]
class JsInteropMessageHandler : HttpMessageHandler
{

    private readonly ILogger<JsInteropMessageHandler> _logger;

    public JsInteropMessageHandler(ILogger<JsInteropMessageHandler> logger)
    {
        _logger = logger;
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {

        using var scope = SuppressInstrumentationScope.Begin();
        try {
            using var ms = new MemoryStream();
            request?.Content?.CopyTo(ms, null, cancellationToken);
            ms.Position = 0;
             OtlpExporterInterop.SendExportRequest(ms.ToArray());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _logger.LogWarning(ex, "Error has occured exporting trace");
        }
        return new HttpResponseMessage(HttpStatusCode.OK);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Send(request, cancellationToken));
    }
}

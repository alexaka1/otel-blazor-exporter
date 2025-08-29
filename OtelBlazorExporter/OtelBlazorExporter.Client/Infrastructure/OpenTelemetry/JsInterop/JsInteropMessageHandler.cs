using System.Net;
using System.Runtime.Versioning;
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
            request.Content?.CopyTo(ms, null, cancellationToken);
            ms.Position = 0;
            var data = ms.ToArray();
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            try
            {
                if (path.EndsWith("/v1/metrics", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("Exporting metrics via JS interop size={Size} bytes", data.Length);
                    OtlpExporterInterop.SendMetricsExportRequest(data);
                }
                else if (path.EndsWith("/v1/traces", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("Exporting traces via JS interop size={Size} bytes", data.Length);
                    OtlpExporterInterop.SendTraceExportRequest(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "JS interop export failed for {Path}", path);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred exporting telemetry");
        }
        return new HttpResponseMessage(HttpStatusCode.OK);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Send(request, cancellationToken));
    }
}

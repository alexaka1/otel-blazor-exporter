using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace OtelBlazorExporter.Client.Infrastructure.OpenTelemetry.JsInterop;

[SupportedOSPlatform("browser")]
internal static partial class OtlpExporterInterop
{
    // Imports the JS function exported from wwwroot/js/otlp-export.js module name loaded in Program.cs
    [JSImport("sendTraceExportRequest", "OtelBlazorExporter.Client.OtlpExport")]
    internal static partial void SendTraceExportRequest(byte[] data);

    [JSImport("sendMetricsExportRequest", "OtelBlazorExporter.Client.OtlpExport")]
    internal static partial void SendMetricsExportRequest(byte[] data);
}

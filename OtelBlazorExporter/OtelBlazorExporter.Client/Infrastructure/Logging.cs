using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Serilog;
using Serilog.Debugging;

namespace OtelBlazorExporter.Client.Infrastructure;

public static class Logging
{
    public static void InitStartupLogger()
    {
        // Startup logger
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("SourceContext", "Startup")
            .MinimumLevel.Warning()
            .Enrich.FromLogContext()
            .WriteTo.BrowserConsole()
            .CreateBootstrapLogger();
    }

    public static void AddLogging(this WebAssemblyHostBuilder builder)
    {
        if (builder.HostEnvironment.IsDevelopment())
        {
            SelfLog.Enable(Console.Error);
        }

        builder.Services.AddSerilog((sp, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(sp.GetRequiredService<IConfiguration>())
                .WriteTo.Logger(l =>
                    l.Filter.ByExcluding("SourceContext = 'Microsoft.Hosting.Lifetime'").WriteTo
                        // https://github.com/serilog/serilog-sinks-browserconsole/issues/20
                        .BrowserConsole(jsRuntime: sp.GetRequiredService<IJSRuntime>()))
                ;
        }, writeToProviders: true);
    }
}

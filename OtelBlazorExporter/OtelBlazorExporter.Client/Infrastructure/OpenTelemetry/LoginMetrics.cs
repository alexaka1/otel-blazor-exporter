using System.Diagnostics.Metrics;

namespace OtelBlazorExporter.Client.Infrastructure.OpenTelemetry;

public class LoginMetrics(OtelBlazorMeterProvider meterProvider)
{
    private readonly Counter<long> _jwtValidations =
        meterProvider.Meter.CreateCounter<long>("backend.access_token.validation.failures", "{failures}",
            "The number of token validations that failed");

    private readonly Counter<long> _loginAttempts =
        meterProvider.Meter.CreateCounter<long>("backend.login.attempts", "{logins}", "The number of login attempts");

    public void IncrementLoginAttempts(params ReadOnlySpan<KeyValuePair<string, object?>> tags)
    {
        _loginAttempts.Add(1, tags);
    }

    public void SuccessfulLogin()
    {
        _loginAttempts.Add(1, new KeyValuePair<string, object?>("success", true));
    }

    public void InvalidLogin()
    {
        _loginAttempts.Add(1, new KeyValuePair<string, object?>("success", false));
    }

    public void TokenValidationFailed()
    {
        _jwtValidations.Add(1);
    }
}

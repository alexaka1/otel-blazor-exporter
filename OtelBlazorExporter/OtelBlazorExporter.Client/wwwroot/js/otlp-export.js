function sendGenericExport(bytes, kind) {
  try {
    const u8 = bytes instanceof Uint8Array ? bytes : new Uint8Array(bytes);
    const blob = new Blob([u8], { type: 'application/x-protobuf' });
    const url = `http://localhost:4318/v1/${kind}`;
    if (!(navigator.sendBeacon && navigator.sendBeacon(url, blob))) {
      fetch(url, { method: 'POST', body: blob, headers: { 'Content-Type': 'application/x-protobuf' }, keepalive: true });
    }
  } catch (e) {
    console.error('sendGenericExport failed', kind, e);
  }
}

export function sendTraceExportRequest(bytes) {
  // traces
  sendGenericExport(bytes, 'traces');
}

export function sendMetricsExportRequest(bytes) {
  // metrics
  sendGenericExport(bytes, 'metrics');
}
/*
needs CORS setup for your collector, ie:
```yaml
receivers:
  otlp:
    protocols:
      grpc:
        endpoint: 0.0.0.0:4317
      http:
        endpoint: 0.0.0.0:4318
        cors:
          allowed_origins:
            - "https://localhost:7185"
        allowed_headers:
          - "Content-Type"
          - "Authorization"
        max_age: 7200
```
*/

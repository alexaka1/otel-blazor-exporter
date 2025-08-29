export function sendExportRequest(bytes) {
  try {
    // bytes should arrive as a Uint8Array when imported via [JSImport]
    console.log('sendExportRequest called');
    const u8 = bytes instanceof Uint8Array ? bytes : new Uint8Array(bytes);
    const blob = new Blob([u8], { type: 'application/x-protobuf' });
    navigator.sendBeacon('http://localhost:4318/v1/traces', blob);
  } catch (e) {
    console.error('sendExportRequest failed', e);
  }
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

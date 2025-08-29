export function sendExportRequest(bytes) {
  try {
    // bytes should arrive as a Uint8Array when imported via [JSImport]
    console.log('sendExportRequest');
    console.log(bytes);
    const u8 = bytes instanceof Uint8Array ? bytes : new Uint8Array(bytes);
    const blob = new Blob([u8], { type: 'application/x-protobuf' });
    console.log(blob);
    navigator.sendBeacon('http://localhost:4318/v1/traces', blob);
  } catch (e) {
    console.error('sendExportRequest failed', e);
  }
}

export class WebAuthnUtils {
    public static bufferToBase64url(buffer: ArrayBuffer): string {
        const bytes = new Uint8Array(buffer);
        let str = '';
        for (const charCode of bytes) {
            str += String.fromCharCode(charCode);
        }
        const base64String = btoa(str);
        return base64String.replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
    }

    public static base64urlToBuffer(base64url: string): ArrayBuffer {
        const padding = '==='.slice((base64url.length + 3) % 4);
        const base64 = (base64url + padding).replace(/-/g, '+').replace(/_/g, '/');
        const str = atob(base64);
        const buffer = new ArrayBuffer(str.length);
        const bytes = new Uint8Array(buffer);
        for (let i = 0; i < str.length; i++) {
            bytes[i] = str.charCodeAt(i);
        }
        return buffer;
    }
}

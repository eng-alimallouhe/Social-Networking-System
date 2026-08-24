export function getDeviceIcon(os: string, deviceName: string): any {
    const combined = `${os} ${deviceName}`.toLowerCase();
    if (combined.includes('ipad') || combined.includes('tablet')) {
        return 'tablet';
    }
    if (combined.includes('android') || combined.includes('iphone') || combined.includes('mobile') || combined.includes('ios')) {
        return 'smartphone';
    }
    return 'laptop';
}

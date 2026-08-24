import { ActiveSessionDto } from './active-session.dto';
import { RegisteredDeviceDto } from './registered-device.dto';

export interface UserActiveSessionsAndDevicesResult {
    activeSessions: ActiveSessionDto[];
    registeredDevices: RegisteredDeviceDto[];
}

import { StatusCode } from '../contracts/status-code';

const userStatus = (code: number): StatusCode => ({
    category: 'User',
    code
});

export class UserStatusCodes {
    static readonly HighRiskLoginAttempt = userStatus(401);
    static readonly Created = userStatus(201);
    static readonly Unauthorized = userStatus(400);
    static readonly Found = userStatus(200);
    static readonly UserNameAvailable = userStatus(2001);
    static readonly NotFound = userStatus(404);
    static readonly AlreadyExists = userStatus(4091);
    static readonly Conflict = userStatus(4092);
    static readonly Deactivated = userStatus(403);
    static readonly LockedOut = userStatus(429);
    static readonly Banned = userStatus(4031);
    static readonly Suspended = userStatus(4032);
    static readonly NotVerified = userStatus(4033);
    static readonly ProfileNotCompleted = userStatus(4034);
    static readonly UserNameAlreadyExists = userStatus(4036);
    static readonly InvalidSecurityUse = userStatus(4041);
    static readonly FailedLoginAttempt = userStatus(4050);
    static readonly MaxLoginAttempts = userStatus(4051);
}
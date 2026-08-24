import { StatusCode } from '../../contracts/status-code';

const securityStatus = (code: number): StatusCode => ({
    category: 'Security',
    code
});

export class SecurityStatusCodes {
    static readonly AuthenticationRequired = securityStatus(401);

    static readonly Unauthorized = securityStatus(403);

    static readonly VerificationFailed = securityStatus(4041);

    static readonly TfaRequired = securityStatus(2001);

    static readonly MfaRequired = securityStatus(2002);

    static readonly MfaAlreadyEnabled = securityStatus(4043);

    static readonly InvalidMfaCode = securityStatus(4044);

    static readonly CriticalLoginRisk = securityStatus(406);

    static readonly RoleNotFound = securityStatus(407);

    static readonly AccessDenied = securityStatus(409);

    static readonly RequestRejected = securityStatus(410);

    static readonly TokenGenerationError = securityStatus(5001);

    static readonly RecoveryEmailNotLinked = securityStatus(4091);

    static readonly AuthenticatorAppNotLinked = securityStatus(4092);

    static readonly PasskeyNotAdded = securityStatus(4093);
}
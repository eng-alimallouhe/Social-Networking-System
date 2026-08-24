import { StatusCode } from '../../contracts/status-code';

const verificationStatus = (code: number): StatusCode => ({
    category: 'Verification',
    code
});

export class VerificationStatusCodes {
    static readonly CodeSent = verificationStatus(201);

    static readonly CodeVerified = verificationStatus(200);

    static readonly CodeResent = verificationStatus(202);

    static readonly NoActiveCode = verificationStatus(404);

    static readonly CodeExpired = verificationStatus(410);

    static readonly InvalidCode = verificationStatus(400);

    static readonly MaxAttemptsReached = verificationStatus(429);

    static readonly Throttled = verificationStatus(4291);
}
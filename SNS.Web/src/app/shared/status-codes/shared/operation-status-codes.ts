import { StatusCode } from '../../contracts/status-code';

const operationStatus = (code: number): StatusCode => ({
    category: 'Operation',
    code
});

export class OperationStatusCodes {
    static readonly Success = operationStatus(200);

    static readonly Failure = operationStatus(400);

    static readonly AuthenticationRequired = operationStatus(401);

    static readonly AccessDenied = operationStatus(403);

    static readonly ResourceNotFound = operationStatus(404);

    static readonly Conflict = operationStatus(409);

    static readonly InvalidInput = operationStatus(422);

    static readonly TokenInvalid = operationStatus(4221);

    static readonly ServerError = operationStatus(500);

    static readonly ExpiredInfo = operationStatus(4091);
}
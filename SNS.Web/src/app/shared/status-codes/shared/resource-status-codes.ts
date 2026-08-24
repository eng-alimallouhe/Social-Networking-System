import { StatusCode } from '../../contracts/status-code';

const resourceStatus = (code: number): StatusCode => ({
    category: 'Resource',
    code
});

export class ResourceStatusCodes {
    static readonly Found = resourceStatus(200);

    static readonly NotFound = resourceStatus(404);

    static readonly ReadError = resourceStatus(500);
}
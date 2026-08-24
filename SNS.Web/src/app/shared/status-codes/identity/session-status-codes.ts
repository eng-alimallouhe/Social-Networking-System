import { StatusCode } from "../../contracts/status-code";

const sessionStatus = (code: number): StatusCode => ({
    category: 'Session',
    code
});

export class SessionStatusCodes {
    static readonly NotFound = sessionStatus(404);
}
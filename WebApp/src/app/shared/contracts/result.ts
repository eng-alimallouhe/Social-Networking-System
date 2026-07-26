import { StatusCode } from "./status-code";

export interface Result<T = void> {
    isSuccess: boolean;
    statusCode: StatusCode;
    value: T | null;
}
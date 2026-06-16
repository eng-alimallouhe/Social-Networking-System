export interface StatusCode {
    category: string;
    code: number;
}

export interface Result<T = void> {
    isSuccess: boolean;
    statusCode: StatusCode;
    value?: T;
}
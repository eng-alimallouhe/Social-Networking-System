export interface StatusCode {
    category: string;
    code: number
}


export function isStatusCode(
    actual: StatusCode,
    expected: StatusCode
): boolean {
    return actual.category === expected.category
        && actual.code === expected.code;
};
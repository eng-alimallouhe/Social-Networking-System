export interface InitiatePasskeyRegistrationCommand {
    attestationType?: string;
}

export interface CredentialCreateOptionsDto {
    rp: {
        id: string;
        name: string;
    };
    user: {
        id: string; // Base64Url encoded
        name: string;
        displayName: string;
    };
    challenge: string; // Base64Url encoded
    pubKeyCredParams: {
        type: string;
        alg: number;
    }[];
    timeout?: number;
    attestation?: string;
    authenticatorSelection?: {
        authenticatorAttachment?: string;
        requireResidentKey?: boolean;
        residentKey?: string;
        userVerification?: string;
    };
    excludeCredentials?: {
        id: string; // Base64Url encoded
        type: string;
    }[];
    extensions?: any;
}

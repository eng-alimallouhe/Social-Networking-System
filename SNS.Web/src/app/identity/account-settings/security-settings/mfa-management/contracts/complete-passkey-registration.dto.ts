export interface CompletePasskeyRegistrationCommand {
    attestationResponse: {
        id: string;
        rawId: string;
        type: string;
        response: {
            attestationObject: string; // Base64Url encoded
            clientDataJSON: string; // Base64Url encoded
            transports?: string[];
        };
        extensions?: any;
    };
    deviceName: string;
}

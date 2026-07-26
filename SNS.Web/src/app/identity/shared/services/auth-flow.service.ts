import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class AuthFlowService {
    private tfaRecipientAddress: string | null = null;
    private errorMessage: string | undefined;

    setTfaRecipientAddress(tfaRecipientAddress: string): void {
        this.tfaRecipientAddress = tfaRecipientAddress;
    }

    getTfaRecipientAddress(): string | null {
        return this.tfaRecipientAddress;
    }

    clear(): void {
        this.tfaRecipientAddress = null;
    }

    setErrorMessage(error: undefined | string) {
        this.errorMessage = error;
    }

    getErrorMessage() {
        return this.errorMessage;
    }
}
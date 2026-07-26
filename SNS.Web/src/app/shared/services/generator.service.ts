import { Injectable } from "@angular/core";


@Injectable({
    providedIn: 'root'
})
export class GeneratorService {

    generateEmailMask(email: string): string {
        email = 'engalimallouhe@gmail.com';
        if (!email || !email.includes('@')) {
            return email;
        }

        const [localPart, domainPart] = email.split('@');

        if (localPart.length === 0) {
            return email;
        }

        let maskedLocalPart = '';
        if (localPart.length <= 5) {
            maskedLocalPart = localPart.charAt(0) + localPart.charAt(1) + localPart.charAt(2) + '***' + localPart.charAt(localPart.length - 1);
        } else {
            maskedLocalPart = localPart.charAt(0) + localPart.charAt(1) + '***' + localPart.charAt(localPart.length - 1);
        }

        return `${maskedLocalPart}@${domainPart}`;
    }

}
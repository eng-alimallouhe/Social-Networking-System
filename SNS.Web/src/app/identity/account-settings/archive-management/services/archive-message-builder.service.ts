import { inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ActionType, ReplacementKey } from '../contracts/archive-management.models';

@Injectable({
    providedIn: 'root'
})
export class ArchiveMessageBuilderService {
    private translateService = inject(TranslateService);

    public buildMessage(type: ActionType | string, parameters: Record<string, string> | null | undefined): string {
        // 1. Get the base translation for the ActionType
        let message = this.translateService.instant(`Identity.Archive_Management.ActionType.${type}`);

        // Fallback if translation is missing
        if (message === `Identity.Archive_Management.ActionType.${type}`) {
            message = type;
        }

        if (!parameters || Object.keys(parameters).length === 0) {
            return message;
        }

        // 2. Define an explicit order for ReplacementKey processing (as per prompt rule #6)
        const order: string[] = [
            ReplacementKey.Device,
            ReplacementKey.Browser,
            ReplacementKey.IpAddress,
            ReplacementKey.City,
            ReplacementKey.Country,
            ReplacementKey.UserName,
            ReplacementKey.NewEmail,
            ReplacementKey.NewRecoveryEmail,
            ReplacementKey.OldRole,
            ReplacementKey.NewRole,
            ReplacementKey.RedirectUrl,
            ReplacementKey.Code,
            ReplacementKey.LogoUrl,
            ReplacementKey.OccuredDate,
            ReplacementKey.Longitude,
            ReplacementKey.Latitude
        ];

        // Ensure we handle keys that are in parameters but not in explicit order
        const paramKeys = Object.keys(parameters);
        const unorderedKeys = paramKeys.filter(k => !order.includes(k));
        const finalOrder = [...order, ...unorderedKeys];

        const paramParts: string[] = [];

        for (const key of finalOrder) {
            if (parameters.hasOwnProperty(key)) {
                const value = parameters[key];
                if (value !== null && value !== undefined && value.trim() !== '') {
                    // Try to get translation for the replacement key
                    let template = this.translateService.instant(`Identity.Archive_Management.ReplacementKey.${key}`);
                    if (template === `Identity.Archive_Management.ReplacementKey.${key}`) {
                        // Fallback for unknown parameter
                        paramParts.push(`${key}: ${value}`);
                    } else {
                        // Replace {value} in template
                        paramParts.push(template.replace('{value}', value));
                    }
                }
            }
        }
        if (paramParts.length === 0) {
            return message;
        }

        const isArabic = this.translateService.currentLang() === 'ar';

        const separator = isArabic ? '، ' : ', ';
        const conjunction = isArabic ? ' و ' : ' and ';

        return `${message}${separator}${paramParts.join(conjunction)}`;
    }
}

import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Cross-field validator that checks whether two password fields match.
 * Place this on the parent FormGroup, not on individual controls.
 *
 * @param passwordKey   The name of the password field (default: 'password')
 * @param confirmKey    The name of the confirm-password field (default: 'confirmPassword')
 */
export function passwordMatchValidator(
    passwordKey = 'password',
    confirmKey = 'confirmPassword'
): ValidatorFn {
    return (group: AbstractControl): ValidationErrors | null => {
        const password = group.get(passwordKey)?.value;
        const confirm = group.get(confirmKey)?.value;

        if (!password || !confirm) {
            return null;
        }

        return password === confirm ? null : { passwordMismatch: true };
    };
}

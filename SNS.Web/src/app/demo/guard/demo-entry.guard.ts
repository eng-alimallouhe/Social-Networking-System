import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../../identity/shared/services/authentication.service';

export const demoEntryGuard: CanActivateFn = () => {
    const authService = inject(AuthenticationService);
    const router = inject(Router);
    console.log('test');

    if (authService.isAuthenticated()) {
        return router.createUrlTree(['/demo/dashboard']);
    }

    return router.createUrlTree(['/demo/role-switcher']);
};
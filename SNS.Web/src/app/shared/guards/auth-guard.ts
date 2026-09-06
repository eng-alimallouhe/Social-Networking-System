import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthenticationService } from '../../identity/shared/services/authentication.service';

export const authGuard: CanActivateFn = (route, state) => {

  const authenticationService = inject(AuthenticationService);
  const router = inject(Router);

  if (!!authenticationService.getAccessToken()) {
    return true;
  }

  return router.createUrlTree(['/auth/login']);
};

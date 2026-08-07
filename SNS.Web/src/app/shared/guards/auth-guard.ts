import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../../identity/shared/services/token.service';

export const authGuard: CanActivateFn = (route, state) => {

  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (!!tokenService.getAccessToken()) {
    return true;
  }

  return router.createUrlTree(['/auth/login']);
};

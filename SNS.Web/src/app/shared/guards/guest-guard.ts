import { CanActivateChildFn, Router } from "@angular/router";
import { TokenService } from "../../identity/shared/services/token.service";
import { inject } from "@angular/core";

export const guestGuard: CanActivateChildFn = () => {

  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (!!tokenService.getAccessToken()) {
    return router.createUrlTree(['/']);
  }

  return true;
};
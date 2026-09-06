import { CanActivateChildFn, Router } from "@angular/router";
import { AuthenticationService } from "../../identity/shared/services/authentication.service";
import { inject } from "@angular/core";

export const guestGuard: CanActivateChildFn = () => {

  const authenticationService = inject(AuthenticationService);
  const router = inject(Router);

  if (!!authenticationService.getAccessToken()) {
    return router.createUrlTree(['/']);
  }

  return true;
};
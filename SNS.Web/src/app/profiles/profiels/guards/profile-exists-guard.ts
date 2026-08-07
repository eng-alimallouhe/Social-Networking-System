import { CanActivateFn } from '@angular/router';

export const profileExistsGuard: CanActivateFn = (route, state) => {
  return true;
};

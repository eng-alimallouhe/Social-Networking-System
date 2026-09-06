import { inject, Injectable } from '@angular/core';
import { AuthenticationService } from '../../identity/shared/services/authentication.service';

@Injectable({
  providedIn: 'root'
})
export class DemoDataService {
  private authenticationService = inject(AuthenticationService);

  public generateGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

  public getDemoEmail(): string {
    const token = this.authenticationService.getAccessToken();
    if (token) {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return payload.email || payload.sub || 'admin@demo.com';
        } catch { }
    }
    return 'admin@demo.com';
  }

  public getDemoUserId(): string {
      const token = this.authenticationService.getAccessToken();
      if (token) {
          try {
              const payload = JSON.parse(atob(token.split('.')[1]));
              return payload.nameid || payload.uid || payload.sub || this.generateGuid();
          } catch { }
      }
      return this.generateGuid();
  }

  public generateChallengeToken(): string {
      return btoa(this.generateGuid() + '-' + Date.now().toString());
  }
}

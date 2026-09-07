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

  public getDemoProfileId(): string {
      return this.authenticationService.getProfileId() || this.authenticationService.getUserId() || '4c0f91e8-55f1-444b-b430-3c82cf75d950';
  }

  public getDemoCompanyId(): string {
      return 'b1c37f36-38c6-41bf-9e78-5e2260bde7c5';
  }

  public getDemoCommunityId(): string {
      return 'a8abb121-698e-4d16-ba9d-04d3056b347a';
  }

  public getDemoJobId(): string {
      return '2edb6e7e-521b-4906-860a-e52867efbf05';
  }

  public generateChallengeToken(): string {
      return btoa(this.generateGuid() + '-' + Date.now().toString());
  }
}

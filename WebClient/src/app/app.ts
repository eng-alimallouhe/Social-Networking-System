import { Component, signal, afterNextRender, ElementRef, viewChild, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthenticationService } from './Identity/Authentication/Services/authentication.service';
import { ProfileService } from './social-graph/services/profile.service';
import { environment } from '../environments/environment.development';
import { TokenService } from './Identity/Shared/Services/token.service';
import { ToastContainerComponent } from "./shared/Components/toast/toast-container/toast-container.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ToastContainerComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private authenticationService = inject(AuthenticationService);
  private profileService = inject(ProfileService);
  private tokenService = inject(TokenService);

  protected readonly title = signal('Syrian Developers Network');

  ngOnInit(): void {
    this.authenticationService.checkIfUserIsAuthenticated();
    if (this.authenticationService.isAuthenticated()) {
      this.profileService.loadProfile();
    }
    if (!environment.production) {
      console.warn("⚠️ DEV MODE: Injecting fake token and bypassing auth");
      this.authenticationService.isAuthenticated.set(true);
      const accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsIlN1YiI6Ijk0NzNhODllLThkOWEtNDc0MC1iOGE4LWFkMGNhMjhiMTY2MiIsIkp0aSI6Ijk0NzNhODllLThkOWEtNDc0MC1iOGE4LWFkMGNhMjhiMTY2MiIsInNlc3Npb25JZCI6Ijk0NzNhODllLThkOWEtNDc0MC1iOGE4LWFkMGNhMjhiMTY2MiIsInByb2ZpbGVJZCI6Ijk0NzNhODllLThkOWEtNDc0MC1iOGE4LWFkMGNhMjhiMTY2MiIsInJvbGUiOiJ1c2VyIn0.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0.t59B9tSNDBqvwWKNw2THwHRBB62-Q1o9q53O2_2bZeA";
      const refreshToken = "9473289e8d9a4740b8a83213210ca28b1662";
      this.tokenService.setToken(accessToken, refreshToken);
    }
  }
}
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideChevronRight } from '@lucide/angular';
import { UserSecuritySettingsService } from '../../services/user-security-settings.service';
import { LoadingSettingsService } from '../../../shared/services/loading-settings.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-authenticator-setup',
  imports: [
    CommonModule,
    TranslatePipe,
    LucideChevronRight,
    RouterLink
  ],
  templateUrl: './authenticator-setup.html',
  styleUrl: './authenticator-setup.css'
})
export class AuthenticatorSetup implements OnInit {
  private router = inject(Router);
  private securityService = inject(UserSecuritySettingsService);
  private loadingService = inject(LoadingSettingsService);

  secretKey = signal<string>('');
  qrCodeUri = signal<string>('');
  isLoaded = signal<boolean>(false);

  get qrCodeImageUrl(): string {
    if (!this.qrCodeUri()) return '';
    return `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(this.qrCodeUri())}`;
  }

  ngOnInit() {
    this.loadingService.show();
    this.securityService.initiateAuthenticatorRegistration()
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess && res.value) {
            this.secretKey.set(res.value.secretKey);
            this.qrCodeUri.set(res.value.qrCodeUri);
            this.isLoaded.set(true);
          }
        }
      });
  }

  onContinue(): void {
    if (this.isLoaded()) {
      this.router.navigate(['/account-settings/security-settings/verify-authenticator']);
    }
  }
}

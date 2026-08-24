import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideScanQrCode, LucideArrowRight, LucideCopy, LucideCheck } from '@lucide/angular';
import { AppTooltipDirective } from '../../../../../../shared/design-system/components/app-tooltip/app-tooltip.directive';
import { finalize } from 'rxjs';
import { MfaManagementService } from '../../services/mfa-management.service';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';

@Component({
  selector: 'app-authenticator-setup',
  imports: [
    CommonModule,
    TranslatePipe,
    LucideScanQrCode,
    LucideArrowRight,
    LucideCopy,
    LucideCheck,
    AppTooltipDirective,
    RouterLink
  ],
  templateUrl: './authenticator-setup.html',
  styleUrl: './authenticator-setup.css'
})
export class AuthenticatorSetup implements OnInit {
  private router = inject(Router);
  private mfaManagementService = inject(MfaManagementService);
  private loadingService = inject(GlobalLoaderService);

  secretKey = signal<string>('');
  qrCodeUri = signal<string>('');
  isLoaded = signal<boolean>(false);
  isCopied = signal<boolean>(false);

  get qrCodeImageUrl(): string {
    if (!this.qrCodeUri()) return '';
    return `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(this.qrCodeUri())}`;
  }

  ngOnInit() {
    this.loadingService.show();
    this.mfaManagementService.initiateAuthenticatorRegistration()
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

  copyToClipboard(): void {
    const key = this.secretKey();
    if (key && !this.isCopied()) {
      navigator.clipboard.writeText(key).then(() => {
        this.isCopied.set(true);
        setTimeout(() => {
          this.isCopied.set(false);
        }, 2000);
      }).catch(err => {
        console.error('Failed to copy secret key', err);
      });
    }
  }
}

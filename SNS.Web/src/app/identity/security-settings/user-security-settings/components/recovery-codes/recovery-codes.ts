import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';
import { LucideChevronLeft, LucideCopy, LucideDownload, LucideRefreshCw } from '@lucide/angular';
import { UserSecuritySettingsService } from '../../services/user-security-settings.service';
import { LoadingSettingsService } from '../../../shared/services/loading-settings.service';
import { AppConfirmDialog } from '../../../../../shared/components/app-confirm-dialog/app-confirm-dialog';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-recovery-codes',
  imports: [
    CommonModule,
    TranslatePipe,
    LucideChevronLeft,
    LucideCopy,
    LucideDownload,
    LucideRefreshCw,
    RouterLink,
    AppConfirmDialog
  ],
  templateUrl: './recovery-codes.html',
  styleUrl: './recovery-codes.css'
})
export class RecoveryCodes implements OnInit {
  private securityService = inject(UserSecuritySettingsService);
  private loadingService = inject(LoadingSettingsService);

  unusedCodesCount = signal<number>(0);
  usedCodesCount = signal<number>(0);
  recoveryCodes = signal<string[]>([]);
  codesExist = computed(() => this.unusedCodesCount() > 0 || this.usedCodesCount() > 0);
  
  showRegenerateConfirm = signal(false);

  ngOnInit() {
    this.loadSettings();
  }

  loadSettings() {
    this.loadingService.show();
    this.securityService.getSecuritySettings()
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess && res.value) {
            this.unusedCodesCount.set(res.value.unusedRecoveryCodesCount);
            this.usedCodesCount.set(res.value.usedRecoveryCodesCount);
            // In a future task, if the backend returned the active codes, we would set them here.
          }
        }
      });
  }

  generateCodes() {
    // Backend endpoint does not exist yet. Only log action or show a message.
    console.log('Generate codes clicked');
  }

  onRegenerateClick() {
    this.showRegenerateConfirm.set(true);
  }

  onCancelRegenerate() {
    this.showRegenerateConfirm.set(false);
  }

  onConfirmRegenerate() {
    this.showRegenerateConfirm.set(false);
    // Future backend call to regenerate recovery codes
    console.log('Regenerate codes confirmed');
  }

  copyToClipboard() {
    if (this.recoveryCodes().length === 0) return;
    navigator.clipboard.writeText(this.recoveryCodes().join('\n'));
  }

  downloadTxt() {
    if (this.recoveryCodes().length === 0) return;
    const element = document.createElement('a');
    const file = new Blob([this.recoveryCodes().join('\n')], { type: 'text/plain' });
    element.href = URL.createObjectURL(file);
    element.download = "recovery-codes.txt";
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
  }
}

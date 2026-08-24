import { Component, inject, OnInit, signal, computed, effect } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';
import { LucideCopy, LucideDownload, LucideRefreshCw, LucideTrash2, LucideCheck } from '@lucide/angular';
import { AppConfirmDialog } from '../../../../../../shared/design-system/components/app-confirm-dialog/app-confirm-dialog';
import { ConfirmStateService } from '../../../../../../shared/design-system/services/confirm-state.service';
import { ConfirmAction } from '../../../../../../shared/design-system/services/confirm-action.enum';
import { RecoveryCodeUsingSnapshot } from '../../contracts/user-recovery-codes.dto';
import { finalize } from 'rxjs';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { RecoveryService } from '../../services/recovery.service';
import { SecuritySettingsStateService } from '../../../shared/services/security-settings-state.service';

@Component({
  selector: 'app-recovery-codes',
  imports: [
    CommonModule,
    TranslatePipe,
    LucideCopy,
    LucideDownload,
    LucideRefreshCw,
    LucideTrash2,
    LucideCheck,
    RouterLink,
    AppConfirmDialog,
    DatePipe
  ],
  templateUrl: './recovery-codes.html',
  styleUrl: './recovery-codes.css'
})
export class RecoveryCodes implements OnInit {
  private recoveryService = inject(RecoveryService);
  private loadingService = inject(GlobalLoaderService);
  private confirmStateService = inject(ConfirmStateService);
  private securitySettingsStateService = inject(SecuritySettingsStateService);

  ConfirmAction = ConfirmAction;

  unusedCodesCount = signal<number>(0);
  usedCodesCount = signal<number>(0);
  recoveryCodesHistory = signal<RecoveryCodeUsingSnapshot[]>([]);
  codesExist = computed(() => this.unusedCodesCount() > 0 || this.usedCodesCount() > 0);

  showRegenerateConfirm = signal(false);
  showRevokeConfirm = signal(false);

  newlyGeneratedCodes = signal<string[] | null>(null);
  showGeneratedCodesPanel = computed(() => this.newlyGeneratedCodes() !== null);

  constructor() {
    effect(() => {
      const action = this.confirmStateService.confirmedAction();
      if (action === ConfirmAction.Regenerate) {
        this.confirmStateService.consume();
        this.doGenerateCodes();
      } else if (action === ConfirmAction.Revoke) {
        this.confirmStateService.consume();
        this.doRevokeCodes();
      }
    }, { allowSignalWrites: true });
  }

  ngOnInit() {
    this.loadSettings();
  }

  loadSettings() {
    this.loadingService.show();
    this.recoveryService.getUserRecoveryCodesHistory()
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess && res.value) {
            this.unusedCodesCount.set(res.value.unusedCodesCount);
            this.usedCodesCount.set(res.value.usedCodesCount);
            this.recoveryCodesHistory.set(res.value.recoveryCodesUsingHistory || []);
          }
        }
      });
  }

  generateCodes() {
    this.showRegenerateConfirm.set(true);
  }

  doGenerateCodes() {
    this.loadingService.show();
    this.recoveryService.generateRecoveryCodes()
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess && res.value) {
            this.newlyGeneratedCodes.set(res.value);
            this.securitySettingsStateService.notifySettingsChanged();
            this.loadSettings();
          }
        }
      });
  }

  revokeCodes() {
    this.showRevokeConfirm.set(true);
  }

  doRevokeCodes() {
    this.loadingService.show();
    this.recoveryService.revokeRecoveryCodes()
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.newlyGeneratedCodes.set(null);
            this.securitySettingsStateService.notifySettingsChanged();
            this.loadSettings();
          }
        }
      });
  }

  copyToClipboard() {
    const codes = this.newlyGeneratedCodes();
    if (!codes || codes.length === 0) return;
    navigator.clipboard.writeText(codes.join('\n'));
  }

  downloadJson() {
    const codes = this.newlyGeneratedCodes();
    if (!codes || codes.length === 0) return;
    const jsonContent = JSON.stringify({ recoveryCodes: codes }, null, 2);
    const element = document.createElement('a');
    const file = new Blob([jsonContent], { type: 'application/json' });
    element.href = URL.createObjectURL(file);
    element.download = "recovery-codes.json";
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
  }

  closeGeneratedPanel() {
    this.newlyGeneratedCodes.set(null);
  }
}

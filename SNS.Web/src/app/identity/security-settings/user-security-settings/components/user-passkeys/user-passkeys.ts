import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';
import { LucideChevronLeft, LucidePlus, LucideTrash2, LucideKeyRound } from '@lucide/angular';
import { UserSecuritySettingsService } from '../../services/user-security-settings.service';
import { LoadingSettingsService } from '../../../shared/services/loading-settings.service';
import { PasskeyDto } from '../../contracts/passkey.dto';
import { AppConfirmDialog } from '../../../../../shared/components/app-confirm-dialog/app-confirm-dialog';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-user-passkeys',
  imports: [
    CommonModule,
    TranslatePipe,
    LucideChevronLeft,
    LucidePlus,
    LucideTrash2,
    LucideKeyRound,
    RouterLink,
    AppConfirmDialog
  ],
  templateUrl: './user-passkeys.html',
  styleUrl: './user-passkeys.css'
})
export class UserPasskeys {
  private securityService = inject(UserSecuritySettingsService);
  private loadingService = inject(LoadingSettingsService);

  passkeysList = signal<PasskeyDto[]>([]);
  hasPasskeys = computed(() => this.passkeysList().length > 0);

  showDeleteConfirm = signal(false);
  passkeyToDelete = signal<string | null>(null);

  constructor() {
    this.loadPasskeys();
  }

  loadPasskeys() {
    this.loadingService.show();
    this.securityService.getUserPasskeys()
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess && res.value) {
            this.passkeysList.set(res.value);
          }
        }
      });
  }

  onAddPasskey() {
    // Add passkey logic will be completed in a future task.
    console.log('Add passkey clicked');
  }

  onDeleteClick(passkeyId: string) {
    this.passkeyToDelete.set(passkeyId);
    this.showDeleteConfirm.set(true);
  }

  onCancelDelete() {
    this.passkeyToDelete.set(null);
    this.showDeleteConfirm.set(false);
  }

  onConfirmDelete() {
    const id = this.passkeyToDelete();
    if (!id) return;

    this.showDeleteConfirm.set(false);
    this.loadingService.show();

    this.securityService.removePasskey({ passkeyId: id })
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.loadPasskeys();
          }
        }
      });
  }
}

import { Component, input, model, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ConfirmAction } from '../../services/confirm-action.enum';
import { ConfirmStateService } from '../../services/confirm-state.service';
import { LucideX } from '@lucide/angular';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule, TranslatePipe, LucideX],
  templateUrl: './app-confirm-dialog.html',
  styleUrl: './app-confirm-dialog.css'
})
export class AppConfirmDialog {
  private confirmStateService = inject(ConfirmStateService);

  isOpen = model<boolean>(false);
  
  titleKey = input<string>('');
  messageKey = input<string>('');
  confirmTextKey = input<string>('Status_Codes.Shared.Confirm');
  cancelTextKey = input<string>('Identity.Security_Settings.Personal_Info.Change_Email.Cancel');
  
  action = input.required<ConfirmAction>();

  onCancel() {
    this.isOpen.set(false);
  }

  onConfirm() {
    this.confirmStateService.confirm(this.action());
    this.isOpen.set(false);
  }
}

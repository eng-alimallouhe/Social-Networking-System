import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './app-confirm-dialog.html',
  styleUrl: './app-confirm-dialog.css'
})
export class AppConfirmDialog {
  @Input() titleKey: string = '';
  @Input() messageKey: string = '';
  @Input() confirmTextKey: string = 'Status_Codes.Shared.Confirm';
  @Input() cancelTextKey: string = 'Identity.Security_Settings.Personal_Info.Change_Email.Cancel';
  
  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  onCancel() {
    this.cancel.emit();
  }

  onConfirm() {
    this.confirm.emit();
  }
}

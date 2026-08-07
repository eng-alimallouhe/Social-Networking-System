import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-device-name-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './device-name-dialog.html',
  styleUrl: './device-name-dialog.css'
})
export class DeviceNameDialog {
  @Output() confirm = new EventEmitter<string>();
  @Output() cancel = new EventEmitter<void>();

  deviceName: string = '';

  onCancel() {
    this.cancel.emit();
  }

  onConfirm() {
    if (this.deviceName.trim()) {
      this.confirm.emit(this.deviceName.trim());
    }
  }
}

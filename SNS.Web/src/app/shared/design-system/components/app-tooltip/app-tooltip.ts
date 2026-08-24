import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-tooltip-component',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  template: `
    <div class="app-tooltip">
      {{ messageKey | translate }}
    </div>
  `,
  styleUrl: './app-tooltip.css'
})
export class AppTooltip {
  @Input() messageKey: string = '';
}

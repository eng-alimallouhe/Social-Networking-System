// toast-container.component.ts
import { Component, inject } from '@angular/core';
import { ToastItemComponent } from '../toast/toast-item.component';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  styleUrl: './toast-container.component.css',
  imports: [ToastItemComponent],
  template: `
    <div id="toast-container">
      @for (toast of toastService.toasts(); track toast.id) {
        <app-toast-item 
          [toast]="toast" 
          (remove)="toastService.remove($event)">
        </app-toast-item>
      }
    </div>
  `
})
export class ToastContainerComponent {
  public toastService = inject(ToastService);
}
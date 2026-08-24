import { Injectable, signal } from '@angular/core';
import { ConfirmAction } from './confirm-action.enum';

@Injectable({
  providedIn: 'root'
})
export class ConfirmStateService {
  private readonly confirmedActionSignal = signal<ConfirmAction | null>(null);
  
  readonly confirmedAction = this.confirmedActionSignal.asReadonly();

  confirm(action: ConfirmAction): void {
    this.confirmedActionSignal.set(action);
  }

  consume(): ConfirmAction | null {
    const action = this.confirmedActionSignal();
    if (action) {
      this.confirmedActionSignal.set(null);
    }
    return action;
  }
}

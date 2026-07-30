import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DemoLoadingService {
  private _isLoading = signal<boolean>(false);
  
  public readonly isLoading = this._isLoading.asReadonly();

  public show(): void {
    this._isLoading.set(true);
  }

  public hide(): void {
    this._isLoading.set(false);
  }
}

import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class HomeStateService {
  private _isSideMenuOpen = signal(false);
  
  public readonly isSideMenuOpen = this._isSideMenuOpen.asReadonly();

  openSideMenu() {
    this._isSideMenuOpen.set(true);
  }

  closeSideMenu() {
    this._isSideMenuOpen.set(false);
  }

  toggleSideMenu() {
    this._isSideMenuOpen.update(val => !val);
  }
}

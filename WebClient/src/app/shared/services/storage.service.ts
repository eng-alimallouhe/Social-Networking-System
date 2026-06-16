import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StorageService {
  private storage = localStorage;

  set(key: StorageKey, value: string) {
    this.storage.setItem(key, value);
  }

  get(key: StorageKey) {
    return this.storage.getItem(key);
  }

  remove(key: StorageKey) {
    this.storage.removeItem(key);
  }

  clear() {
    this.storage.clear();
  }
}

export enum StorageKey {
  Theme = 'theme',
  Language = 'language',
  AccessToken = 'access_token',
  RefreshToken = 'refresh_token',
  UserId = 'user_id',
  Profile = 'profile',
}


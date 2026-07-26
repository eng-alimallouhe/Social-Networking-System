import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { StorageKey, StorageService } from '../../shared/services/storage.service';
import { ProfileBaseDto } from '../dtos/profile-base-dto.dto';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private apiUrl = environment.apiUrl + 'social-graph/profile';
  private storageService = inject(StorageService);
  private http = inject(HttpClient);

  userProfile = signal<ProfileBaseDto | null>(null);
  profileId = signal<string | null>(this.getProfileFromStorage()?.id!);

  private getProfileFromStorage(): ProfileBaseDto | null {
    const profileString = this.storageService.get(StorageKey.Profile);
    if (!profileString) return null;
    try {
      return typeof profileString === 'string' ? JSON.parse(profileString) : profileString;
    } catch {
      return null;
    }
  }

  private updateLocalProfile(profile: ProfileBaseDto): void {
    this.userProfile.set(profile);
    this.profileId.set(profile.id);
    this.storageService.set(StorageKey.Profile, JSON.stringify(profile));
  }

  getUserProfileApi(): Observable<ProfileBaseDto> {
    return this.http.get<ProfileBaseDto>(`${this.apiUrl}/me`).pipe(
      tap(profile => {
        this.updateLocalProfile(profile);
      })
    );
  }

  loadProfile(): void {
    const local = this.getProfileFromStorage();
    if (local) {
      this.userProfile.set(local);
      this.profileId.set(local.id);
    }
    this.getUserProfileApi().subscribe();
  }


  clearProfile(): void {
    this.userProfile.set(null);
    this.profileId.set(null);
    this.storageService.remove(StorageKey.Profile);
  }
}
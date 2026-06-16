import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CommunitySummaryDto } from '../dtos/community-summary.dto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CommunityService {
  private apiUrl = environment.apiUrl + 'communities/';
  private http = inject(HttpClient);

  getTrendingCommunities(): Observable<CommunitySummaryDto[]> {
    return this.http.get<CommunitySummaryDto[]>(`${this.apiUrl}trending`);
  }
}

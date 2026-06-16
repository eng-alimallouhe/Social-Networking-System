import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PostSummaryDto } from '../dtos/post-summary.dto';

@Injectable({
  providedIn: 'root',
})
export class PostService {
  private apiUrl = environment.apiUrl + "posts";

  constructor(private http: HttpClient) {

  }

  getUserFeed(): Observable<PostSummaryDto[]> {
    return this.http.get<PostSummaryDto[]>(`${this.apiUrl}/feed`);
  }

  getUserPosts(userId: string): Observable<PostSummaryDto[]> {
    return this.http.get<PostSummaryDto[]>(`${this.apiUrl}/user-posts`);
  }

  getPostById(postId: string): Observable<PostSummaryDto> {
    return this.http.get<PostSummaryDto>(`${this.apiUrl}/post-details/${postId}`);
  }
}

import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { ReactionType } from '../../shared/enums/reactions-type.enum';
import { Observable } from 'rxjs';
import { Result } from '../../../shared/dtos/result.dto';

@Injectable({
  providedIn: 'root',
})
export class PostReactionService {
  private apiUrl = environment.apiUrl + 'posts' + '/{postId}/reactions';

  constructor(private http: HttpClient) { }

  createReaction(postId: string, reaction: ReactionType): Observable<Result<void>> {
    return this.http.post<Result<void>>(this.apiUrl.replace('{postId}', postId), {
      reactionType: reaction
    });
  }

  deleteReaction(postId: string): Observable<Result<void>> {
    return this.http.delete<Result<void>>(this.apiUrl.replace('{postId}', postId));
  }

  updateReaction(postId: string, reaction: ReactionType): Observable<Result<void>> {
    return this.http.put<Result<void>>(this.apiUrl.replace('{postId}', postId), {
      reactionType: reaction
    });
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { DISCUSSIONS_API_ROUTES } from '../../../../shared/constants/api-routes/discussions-api.routes';
import { Result } from '../../../../shared/contracts/result';
import { ProblemTopicDto } from '../contracts/problem-topic.dto';

@Injectable({
    providedIn: 'root',
})
export class ProblemTopicsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getProblemTopics(problemId: string): Observable<Result<ProblemTopicDto[]>> {
        return this.http.get<Result<ProblemTopicDto[]>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemTopics(problemId)}`
        );
    }
}

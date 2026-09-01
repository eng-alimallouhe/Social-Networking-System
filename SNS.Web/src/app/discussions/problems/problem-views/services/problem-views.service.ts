import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { DISCUSSIONS_API_ROUTES } from '../../../../shared/constants/api-routes/discussions-api.routes';
import { Paged } from '../../../../shared/contracts/paged';
import { Result } from '../../../../shared/contracts/result';
import { ProblemViewerDto } from '../contracts/problem-viewer.dto';
import { RecordProblemViewCommand } from '../contracts/record-problem-view.command';

@Injectable({
    providedIn: 'root',
})
export class ProblemViewsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    recordProblemView(
        problemId: string,
        command: RecordProblemViewCommand = {}
    ): Observable<Result> {
        return this.http.post<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemViews(problemId)}`,
            command
        );
    }

    getProblemViewers(
        problemId: string,
        pageSize: number = 10,
        currentPage: number = 1,
        searchTerm?: string
    ): Observable<Result<Paged<ProblemViewerDto>>> {
        let params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        if (searchTerm) {
            params = params.set('searchTerm', searchTerm);
        }

        return this.http.get<Result<Paged<ProblemViewerDto>>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemViewers(problemId)}`,
            { params }
        );
    }
}

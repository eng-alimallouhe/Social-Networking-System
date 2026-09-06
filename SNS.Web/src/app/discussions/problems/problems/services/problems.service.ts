import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { DISCUSSIONS_API_ROUTES } from '../../../../shared/constants/api-routes/discussions-api.routes';
import { Paged } from '../../../../shared/contracts/paged';
import { Result } from '../../../../shared/contracts/result';
import { ChangeProblemStatusCommand } from '../contracts/change-problem-status.command';
import { CreateProblemCommand } from '../contracts/create-problem.command';
import { ProblemDetailsDto } from '../contracts/problem-details.dto';
import { ProblemSummaryDto } from '../contracts/problem-summary.dto';
import { UpdateProblemCommand } from '../contracts/update-problem.command';

@Injectable({
    providedIn: 'root',
})
export class ProblemsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    createProblem(command: CreateProblemCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.Problems}`,
            command
        );
    }

    getProblemById(problemId: string): Observable<Result<ProblemDetailsDto>> {
        return this.http.get<Result<ProblemDetailsDto>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemById(problemId)}`
        );
    }

    getMyProblems(
        pageSize: number = 10,
        currentPage: number = 1,
        searchTerm?: string
    ): Observable<Result<Paged<ProblemSummaryDto>>> {
        let params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        if (searchTerm) {
            params = params.set('searchTerm', searchTerm);
        }

        return this.http.get<Result<Paged<ProblemSummaryDto>>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.MyProblems}`,
            { params }
        );
    }

    getProblemsByAuthor(
        authorId: string,
        pageSize: number = 10,
        currentPage: number = 1,
        searchTerm?: string
    ): Observable<Result<Paged<ProblemSummaryDto>>> {
        let params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        if (searchTerm) {
            params = params.set('searchTerm', searchTerm);
        }

        return this.http.get<Result<Paged<ProblemSummaryDto>>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemsByAuthor(authorId)}`,
            { params }
        );
    }

    getProblemsByCommunity(
        communityId: string,
        pageSize: number = 10,
        currentPage: number = 1,
        searchTerm?: string
    ): Observable<Result<Paged<ProblemSummaryDto>>> {
        let params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        if (searchTerm) {
            params = params.set('searchTerm', searchTerm);
        }

        return this.http.get<Result<Paged<ProblemSummaryDto>>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemsByCommunity(communityId)}`,
            { params }
        );
    }

    updateProblem(problemId: string, command: UpdateProblemCommand): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemById(problemId)}`,
            command
        );
    }

    deleteProblem(problemId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemById(problemId)}`
        );
    }

    changeProblemStatus(problemId: string, command: ChangeProblemStatusCommand): Observable<Result> {
        return this.http.patch<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ChangeProblemStatus(problemId)}`,
            command
        );
    }
}

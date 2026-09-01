import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { DISCUSSIONS_API_ROUTES } from '../../../../shared/constants/api-routes/discussions-api.routes';
import { Paged } from '../../../../shared/contracts/paged';
import { Result } from '../../../../shared/contracts/result';
import { ChangeSolutionStatusCommand } from '../contracts/change-solution-status.command';
import { CreateSolutionCommand } from '../contracts/create-solution.command';
import { SolutionDetailsDto } from '../contracts/solution-details.dto';
import { SolutionSummaryDto } from '../contracts/solution-summary.dto';
import { UpdateSolutionCommand } from '../contracts/update-solution.command';

@Injectable({
    providedIn: 'root',
})
export class SolutionsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    createSolution(command: CreateSolutionCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.Solutions}`,
            command
        );
    }

    getSolutionById(solutionId: string): Observable<Result<SolutionDetailsDto>> {
        return this.http.get<Result<SolutionDetailsDto>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.SolutionById(solutionId)}`
        );
    }

    getMySolutions(
        pageSize: number = 10,
        currentPage: number = 1
    ): Observable<Result<Paged<SolutionSummaryDto>>> {
        const params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        return this.http.get<Result<Paged<SolutionSummaryDto>>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.MySolutions}`,
            { params }
        );
    }

    getSolutionsByAuthor(
        authorId: string,
        pageSize: number = 10,
        currentPage: number = 1
    ): Observable<Result<Paged<SolutionSummaryDto>>> {
        const params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        return this.http.get<Result<Paged<SolutionSummaryDto>>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.SolutionsByAuthor(authorId)}`,
            { params }
        );
    }

    getProblemSolutions(
        problemId: string,
        pageSize: number = 10,
        currentPage: number = 1
    ): Observable<Result<Paged<SolutionSummaryDto>>> {
        const params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        return this.http.get<Result<Paged<SolutionSummaryDto>>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemSolutions(problemId)}`,
            { params }
        );
    }

    updateSolution(solutionId: string, command: UpdateSolutionCommand): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.SolutionById(solutionId)}`,
            command
        );
    }

    deleteSolution(solutionId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.SolutionById(solutionId)}`
        );
    }

    changeSolutionStatus(solutionId: string, command: ChangeSolutionStatusCommand): Observable<Result> {
        return this.http.patch<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ChangeSolutionStatus(solutionId)}`,
            command
        );
    }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { DISCUSSIONS_API_ROUTES } from '../../../../shared/constants/api-routes/discussions-api.routes';
import { Result } from '../../../../shared/contracts/result';
import { VoteType } from '../../../shared/enums/vote-type.enum';
import { AddOrChangeSolutionVoteCommand } from '../contracts/add-or-change-solution-vote.command';
import { SolutionVoteSummaryDto } from '../contracts/solution-vote-summary.dto';

@Injectable({
    providedIn: 'root',
})
export class SolutionVotesService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    addOrChangeVote(solutionId: string, command: AddOrChangeSolutionVoteCommand): Observable<Result> {
        return this.http.post<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.SolutionVotes(solutionId)}`,
            command
        );
    }

    removeVote(solutionId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.SolutionVotes(solutionId)}`
        );
    }

    getVoteSummary(solutionId: string): Observable<Result<SolutionVoteSummaryDto>> {
        return this.http.get<Result<SolutionVoteSummaryDto>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.SolutionVoteSummary(solutionId)}`
        );
    }

    getMyVote(solutionId: string): Observable<Result<VoteType | null>> {
        return this.http.get<Result<VoteType | null>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.MySolutionVote(solutionId)}`
        );
    }
}

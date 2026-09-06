import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { DISCUSSIONS_API_ROUTES } from '../../../../shared/constants/api-routes/discussions-api.routes';
import { Result } from '../../../../shared/contracts/result';
import { VoteType } from '../../../shared/enums/vote-type.enum';
import { AddOrChangeProblemVoteCommand } from '../contracts/add-or-change-problem-vote.command';
import { ProblemVoteSummaryDto } from '../contracts/problem-vote-summary.dto';

@Injectable({
    providedIn: 'root',
})
export class ProblemVotesService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    addOrChangeVote(problemId: string, command: AddOrChangeProblemVoteCommand): Observable<Result> {
        return this.http.post<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemVotes(problemId)}`,
            command
        );
    }

    removeVote(problemId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemVotes(problemId)}`
        );
    }

    getVoteSummary(problemId: string): Observable<Result<ProblemVoteSummaryDto>> {
        return this.http.get<Result<ProblemVoteSummaryDto>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemVoteSummary(problemId)}`
        );
    }

    getMyVote(problemId: string): Observable<Result<VoteType | null>> {
        return this.http.get<Result<VoteType | null>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.MyProblemVote(problemId)}`
        );
    }
}

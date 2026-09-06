import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { DISCUSSIONS_API_ROUTES } from '../../../../shared/constants/api-routes/discussions-api.routes';
import { Result } from '../../../../shared/contracts/result';
import { AddProblemTagCommand } from '../contracts/add-problem-tag.command';
import { ProblemTagDto } from '../contracts/problem-tag.dto';
import { TagDto } from '../../../../shared/contracts/tag.dto';
import { TagsService } from '../../../../shared/services/tags.service';

@Injectable({
    providedIn: 'root',
})
export class ProblemTagsService {
    private http = inject(HttpClient);
    private tagsService = inject(TagsService);
    private baseUrl = environment.apiUrl;

    addProblemTag(problemId: string, command: AddProblemTagCommand): Observable<Result> {
        return this.http.post<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemTags(problemId)}`,
            command
        );
    }

    removeProblemTag(problemId: string, tagId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.RemoveProblemTag(problemId, tagId)}`
        );
    }

    getProblemTags(problemId: string): Observable<Result<ProblemTagDto[]>> {
        return this.http.get<Result<ProblemTagDto[]>>(
            `${this.baseUrl}${DISCUSSIONS_API_ROUTES.ProblemTags(problemId)}`
        );
    }

    getTags(search?: string): Observable<Result<TagDto[]>> {
        return this.tagsService.getTags(search);
    }
}

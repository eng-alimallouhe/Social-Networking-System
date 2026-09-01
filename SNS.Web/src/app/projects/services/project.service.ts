import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PROJECTS_API_ROUTES } from '../../shared/constants/api-routes/projects-api.routes';
import { Paged } from '../../shared/contracts/paged';
import { Result } from '../../shared/contracts/result';
import { ProjectDetailsDto } from '../contracts/project-details.dto';
import { ProjectOverviewDto } from '../contracts/project-summary.dto';
import { ProjectMediaDto } from '../contracts/project-media.dto';
import { ProjectParticipantDetailsDto } from '../contracts/project-participant-details.dto';
import { ProjectRatingDto } from '../contracts/project-rating.dto';
import { ProjectMilestoneDto } from '../contracts/project-milestone.dto';
import { FileNode } from '../contracts/file-node.dto';
import { CreateProjectCommand } from '../contracts/create-project.command';
import { 
    UpdateProjectCommand, 
    UpdateProjectBasicInfoCommand, 
    ChangeProjectStatusCommand, 
    UpdateProjectReadmeCommand 
} from '../contracts/update-project.command';

@Injectable({
    providedIn: 'root',
})
export class ProjectService {
    private http = inject(HttpClient);
    private baseUrl = `${environment.apiUrl}${PROJECTS_API_ROUTES.Projects}`;

    getProjectFeed(currentPage: number = 1, pageSize: number = 10): Observable<Result<ProjectOverviewDto[]>> {
        const params = new HttpParams()
            .set('CurrentPage', currentPage.toString())
            .set('PageSize', pageSize.toString());

        return this.http.get<Result<ProjectOverviewDto[]>>(`${this.baseUrl}/feed`, { params });
    }

    getProjectById(projectId: string): Observable<Result<ProjectDetailsDto>> {
        return this.http.get<Result<ProjectDetailsDto>>(`${this.baseUrl}/${projectId}`);
    }

    getProjectMedia(projectId: string, page: number = 1, pageSize: number = 10): Observable<Result<Paged<ProjectMediaDto>>> {
        const params = new HttpParams()
            .set('CurrentPage', page.toString())
            .set('PageSize', pageSize.toString());

        return this.http.get<Result<Paged<ProjectMediaDto>>>(`${this.baseUrl}/${projectId}/media`, { params });
    }

    getProjectParticipants(projectId: string, page: number = 1, pageSize: number = 10): Observable<Result<Paged<ProjectParticipantDetailsDto>>> {
        const params = new HttpParams()
            .set('CurrentPage', page.toString())
            .set('PageSize', pageSize.toString());

        return this.http.get<Result<Paged<ProjectParticipantDetailsDto>>>(`${this.baseUrl}/${projectId}/participants`, { params });
    }

    getProjectRatings(projectId: string, page: number = 1, pageSize: number = 10): Observable<Result<Paged<ProjectRatingDto>>> {
        const params = new HttpParams()
            .set('CurrentPage', page.toString())
            .set('PageSize', pageSize.toString());

        return this.http.get<Result<Paged<ProjectRatingDto>>>(`${this.baseUrl}/${projectId}/ratings`, { params });
    }

    getProjectMilestones(projectId: string): Observable<Result<ProjectMilestoneDto[]>> {
        return this.http.get<Result<ProjectMilestoneDto[]>>(`${this.baseUrl}/${projectId}/milestones`);
    }

    getProjectSourceCode(projectId: string): Observable<Result<FileNode[]>> {
        return this.http.get<Result<FileNode[]>>(`${this.baseUrl}/${projectId}/source-code`);
    }

    createProject(command: CreateProjectCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(this.baseUrl, command);
    }

    changeProjectStatus(projectId: string, command: ChangeProjectStatusCommand): Observable<Result> {
        return this.http.put<Result>(`${this.baseUrl}/${projectId}/status`, command);
    }

    updateProjectBasicInfo(projectId: string, command: UpdateProjectBasicInfoCommand): Observable<Result> {
        return this.http.put<Result>(`${this.baseUrl}/${projectId}/basic-info`, command);
    }

    updateProject(projectId: string, command: UpdateProjectCommand): Observable<Result<string>> {
        return this.http.put<Result<string>>(`${this.baseUrl}/${projectId}`, command);
    }

    updateProjectReadme(projectId: string, command: UpdateProjectReadmeCommand): Observable<Result> {
        return this.http.put<Result>(`${this.baseUrl}/${projectId}/readme`, command);
    }
}

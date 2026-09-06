import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { PROJECTS_API_ROUTES } from '../../shared/constants/api-routes/projects-api.routes';
import { Result } from '../../shared/contracts/result';
import { Paged } from '../../shared/contracts/paged';
import { 
    AddProjectContributorCommand, 
    ChangeContributorStatusRequest 
} from '../contracts/project-contributor.contracts';
import { ProjectContributorManagementDto } from '../contracts/project-contributor-management.dto';
import { ProfileInvitationCandidateDto } from '../contracts/profile-invitation-candidate.dto';

@Injectable({
    providedIn: 'root'
})
export class ProjectContributorsService {
    private http = inject(HttpClient);

    addProjectContributor(projectId: string, command: AddProjectContributorCommand): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectContributors(projectId)}`;
        return this.http.post<Result>(url, command);
    }

    changeContributorRequestStatus(projectId: string, request: ChangeContributorStatusRequest): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectContributors(projectId)}/status`;
        return this.http.put<Result>(url, request);
    }

    getProjectParticipantsForOwner(projectId: string, page = 1, pageSize = 20): Observable<Result<Paged<ProjectContributorManagementDto>>> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectContributorsManagement(projectId)}`;
        const params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());
        return this.http.get<Result<Paged<ProjectContributorManagementDto>>>(url, { params });
    }

    getProfilesForProjectInvitation(projectId: string, search?: string): Observable<Result<ProfileInvitationCandidateDto[]>> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProfileInvitationCandidates(projectId, search)}`;
        return this.http.get<Result<ProfileInvitationCandidateDto[]>>(url);
    }

    removeProjectContributor(projectId: string, contributorId: string): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectContributorDelete(projectId, contributorId)}`;
        return this.http.delete<Result>(url);
    }
}

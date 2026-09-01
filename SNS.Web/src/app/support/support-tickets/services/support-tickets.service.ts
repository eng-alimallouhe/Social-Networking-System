import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SUPPORT_API_ROUTES } from '../../../shared/constants/api-routes/support-api.routes';
import { Paged } from '../../../shared/contracts/paged';
import { Result } from '../../../shared/contracts/result';
import { SupportTicketCategory } from '../../enums/support-ticket-category.enum';
import { TicketPriority } from '../../enums/ticket-priority.enum';
import { TicketStatus } from '../../enums/ticket-status.enum';
import { AssignSupportTicketRequest } from '../contracts/assign-support-ticket.request';
import { ChangeSupportTicketPriorityRequest } from '../contracts/change-support-ticket-priority.request';
import { ChangeSupportTicketStatusRequest } from '../contracts/change-support-ticket-status.request';
import { CreateSupportTicketRequest } from '../contracts/create-support-ticket.request';
import { SupportTicketDetailsDto } from '../contracts/support-ticket-details.dto';
import { SupportTicketSummaryDto } from '../contracts/support-ticket-summary.dto';

@Injectable({
    providedIn: 'root',
})
export class SupportTicketsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    createTicket(request: CreateSupportTicketRequest): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${SUPPORT_API_ROUTES.Tickets}`,
            request
        );
    }

    getTicketById(ticketId: string): Observable<Result<SupportTicketDetailsDto>> {
        return this.http.get<Result<SupportTicketDetailsDto>>(
            `${this.baseUrl}${SUPPORT_API_ROUTES.TicketById(ticketId)}`
        );
    }

    getMyTickets(
        pageSize: number = 10,
        currentPage: number = 1,
        status?: TicketStatus
    ): Observable<Result<Paged<SupportTicketSummaryDto>>> {
        let params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        if (status) {
            params = params.set('status', status);
        }

        return this.http.get<Result<Paged<SupportTicketSummaryDto>>>(
            `${this.baseUrl}${SUPPORT_API_ROUTES.MyTickets}`,
            { params }
        );
    }

    getSupportTickets(
        pageSize: number = 10,
        currentPage: number = 1,
        status?: TicketStatus,
        priority?: TicketPriority,
        category?: SupportTicketCategory,
        assignedAgentId?: string
    ): Observable<Result<Paged<SupportTicketSummaryDto>>> {
        let params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        if (status) {
            params = params.set('status', status);
        }
        if (priority) {
            params = params.set('priority', priority);
        }
        if (category) {
            params = params.set('category', category);
        }
        if (assignedAgentId) {
            params = params.set('assignedAgentId', assignedAgentId);
        }

        return this.http.get<Result<Paged<SupportTicketSummaryDto>>>(
            `${this.baseUrl}${SUPPORT_API_ROUTES.Tickets}`,
            { params }
        );
    }

    assignTicket(ticketId: string, request: AssignSupportTicketRequest): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${SUPPORT_API_ROUTES.AssignTicket(ticketId)}`,
            request
        );
    }

    changePriority(ticketId: string, request: ChangeSupportTicketPriorityRequest): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${SUPPORT_API_ROUTES.ChangePriority(ticketId)}`,
            request
        );
    }

    changeStatus(ticketId: string, request: ChangeSupportTicketStatusRequest): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${SUPPORT_API_ROUTES.ChangeStatus(ticketId)}`,
            request
        );
    }
}

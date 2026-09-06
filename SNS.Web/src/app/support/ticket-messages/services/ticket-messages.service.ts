import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { SUPPORT_API_ROUTES } from '../../../shared/constants/api-routes/support-api.routes';
import { Result } from '../../../shared/contracts/result';
import { ReplyToSupportTicketRequest } from '../contracts/reply-to-support-ticket.request';
import { TicketMessageDto } from '../contracts/ticket-message.dto';

@Injectable({
    providedIn: 'root',
})
export class TicketMessagesService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    replyToTicket(ticketId: string, request: ReplyToSupportTicketRequest): Observable<Result> {
        return this.http.post<Result>(
            `${this.baseUrl}${SUPPORT_API_ROUTES.TicketMessages(ticketId)}`,
            request
        );
    }

    getTicketMessages(ticketId: string): Observable<Result<TicketMessageDto[]>> {
        return this.http.get<Result<TicketMessageDto[]>>(
            `${this.baseUrl}${SUPPORT_API_ROUTES.TicketMessages(ticketId)}`
        );
    }
}

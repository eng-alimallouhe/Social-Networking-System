import { TicketStatus } from '../../enums/ticket-status.enum';

export interface ChangeSupportTicketStatusRequest {
    status: TicketStatus;
}

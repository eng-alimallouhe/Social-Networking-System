import { TicketPriority } from '../../enums/ticket-priority.enum';

export interface ChangeSupportTicketPriorityRequest {
    priority: TicketPriority;
}

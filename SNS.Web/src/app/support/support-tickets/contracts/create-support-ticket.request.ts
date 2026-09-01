import { SupportTicketCategory } from '../../enums/support-ticket-category.enum';
import { TicketPriority } from '../../enums/ticket-priority.enum';

export interface CreateSupportTicketRequest {
    title: string;
    category: SupportTicketCategory;
    priority: TicketPriority;
    initialMessage: string;
    attachmentObjectKeys?: string[];
}

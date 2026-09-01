import { SupportTicketCategory } from '../../enums/support-ticket-category.enum';
import { TicketPriority } from '../../enums/ticket-priority.enum';
import { TicketStatus } from '../../enums/ticket-status.enum';

export interface SupportTicketSummaryDto {
    id: string;
    userId: string;
    assignedAgentId: string | null;
    title: string;
    category: SupportTicketCategory;
    priority: TicketPriority;
    status: TicketStatus;
    messagesCount: number;
    createdAt: string;
    updatedAt: string;
}

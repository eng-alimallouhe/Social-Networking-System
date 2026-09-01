import { SupportTicketCategory } from '../../enums/support-ticket-category.enum';
import { TicketPriority } from '../../enums/ticket-priority.enum';
import { TicketStatus } from '../../enums/ticket-status.enum';
import { TicketMessageDto } from '../../ticket-messages/contracts/ticket-message.dto';

export interface SupportTicketDetailsDto {
    id: string;
    userId: string;
    assignedAgentId: string | null;
    title: string;
    category: SupportTicketCategory;
    priority: TicketPriority;
    status: TicketStatus;
    createdAt: string;
    updatedAt: string;
    messages: TicketMessageDto[];
}

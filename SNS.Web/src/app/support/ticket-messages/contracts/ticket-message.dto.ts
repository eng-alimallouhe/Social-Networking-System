import { TicketAttachmentDto } from './ticket-attachment.dto';

export interface TicketMessageDto {
    id: string;
    ticketId: string;
    senderId: string;
    isFromAgent: boolean;
    messageBody: string;
    sentAt: string;
    attachments: TicketAttachmentDto[];
}

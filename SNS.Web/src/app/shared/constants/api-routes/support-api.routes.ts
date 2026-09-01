export const SUPPORT_API_ROUTES = {
    // Support Tickets
    Tickets: 'support/tickets',
    MyTickets: 'support/tickets/my',
    TicketById: (ticketId: string) => `support/tickets/${ticketId}`,
    AssignTicket: (ticketId: string) => `support/tickets/${ticketId}/assign`,
    ChangePriority: (ticketId: string) => `support/tickets/${ticketId}/priority`,
    ChangeStatus: (ticketId: string) => `support/tickets/${ticketId}/status`,

    // Ticket Messages
    TicketMessages: (ticketId: string) => `support/tickets/${ticketId}/messages`,
} as const;

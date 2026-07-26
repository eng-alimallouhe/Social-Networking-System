import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface Toast {
    id: string;
    type: ToastType;
    title: string;
    message: string;
    duration: number;
}

@Injectable({
    providedIn: 'root'
})
export class ToastService {
    public toasts = signal<Toast[]>([]);

    public show(type: ToastType, title: string, message: string, duration: number = 5000) {
        const newToast: Toast = {
            id: generateUUID(),
            type,
            title,
            message,
            duration
        };
        this.toasts.update(currentToasts => [...currentToasts, newToast]);
    }

    public remove(id: string) {
        this.toasts.update(currentToasts => currentToasts.filter(t => t.id !== id));
    }

    public success(title: string, message: string, duration?: number) {
        console.log('Success');
        this.show('success', title, message, duration);
    }

    public error(title: string, message: string, duration?: number) {
        this.show('error', title, message, duration);
    }

    public warning(title: string, message: string, duration?: number) {
        this.show('warning', title, message, duration);
    }

    public info(title: string, message: string, duration?: number) {
        this.show('info', title, message, duration);
    }
}

export function generateUUID(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
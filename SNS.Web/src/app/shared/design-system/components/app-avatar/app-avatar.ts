import { Component, input, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { getInitials } from '../../../utils/avatar-utils';

export type AvatarSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'custom';
export type AvatarShape = 'circle' | 'square';

@Component({
    selector: 'app-avatar',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './app-avatar.html',
    styleUrl: './app-avatar.css'
})
export class AppAvatar {
    src = input<string | null | undefined>(null);
    name = input<string | null | undefined>('');
    alt = input<string>('');
    size = input<AvatarSize>('md');
    shape = input<AvatarShape>('circle');
    customClass = input<string>('');

    imageError = signal<boolean>(false);

    constructor() {
        // Reset image error whenever src changes
        effect(() => {
            this.src();
            this.imageError.set(false);
        });
    }

    hasValidImage = computed(() => {
        const url = this.src();
        return !!url && url.trim().length > 0 && !this.imageError();
    });

    containerClasses = computed(() => {
        const base = `avatar-container size-${this.size()} shape-${this.shape()}`;
        const custom = this.customClass()?.trim();
        return custom ? `${base} ${custom}` : base;
    });

    initials = computed(() => {
        return getInitials(this.name());
    });

    onImageError(): void {
        this.imageError.set(true);
    }
}

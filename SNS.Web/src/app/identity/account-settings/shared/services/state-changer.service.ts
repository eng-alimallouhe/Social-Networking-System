import { Injectable, signal } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class StateChangerService {

    private animationTimeout?: number;

    private get parent(): HTMLElement | null {
        return document.querySelector<HTMLElement>('.parent-element');
    }
    private get authNav(): HTMLElement | null {
        return document.querySelector<HTMLElement>('.auth-nav');
    }

    public startStateChanging(): void {
        this.parent?.classList.add('play-state-changing');
        this.authNav?.classList.add('play-state-changing');
    }

    public stopStateChanging(): void {
        this.parent?.classList.remove('play-state-changing');
        this.authNav?.classList.remove('play-state-changing');
    }

    public playStateChangingAnimation(duration: number): void {

        const parent = this.parent;
        if (!parent) return;

        clearTimeout(this.animationTimeout);

        parent.classList.add('play-state-changing');

        this.animationTimeout = window.setTimeout(() => {
            parent.classList.remove('play-state-changing');
        }, duration);
    }
}
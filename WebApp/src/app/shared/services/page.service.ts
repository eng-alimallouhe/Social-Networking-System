import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class PageService {

    private lockCount = 0;

    disableScroll(): void {
        this.lockCount++;

        if (this.lockCount === 1) {
            document.body.classList.add('no-scroll');
        }
    }

    enableScroll(): void {
        if (this.lockCount > 0) {
            this.lockCount--;
        }

        if (this.lockCount === 0) {
            document.body.classList.remove('no-scroll');
        }
    }
}
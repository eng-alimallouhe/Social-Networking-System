import { inject, Injectable, signal } from "@angular/core";
import { PageService } from "../../services/page.service";

@Injectable({
    providedIn: 'root'
})
export class GlobalLoaderService {
    private pageService = inject(PageService);
    private _isLoading = signal(false);
    public isLoading = this._isLoading.asReadonly();

    public show() {
        this._isLoading.set(true);
        this.pageService.disableScroll();
    }

    public hide() {
        this._isLoading.set(false);
        this.pageService.enableScroll();
    }
}
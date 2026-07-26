import { inject, Injectable, signal } from "@angular/core";
import { PageService } from "../../../../shared/services/page.service";

@Injectable({ providedIn: 'root' })
export class LoadingAuthService {
    private pageService = inject(PageService);

    private _isLoadingAuthResponse = signal(false);
    public isLoading = this._isLoadingAuthResponse.asReadonly();

    show() {
        this.pageService.disableScroll();
        this._isLoadingAuthResponse.set(true);
    }

    hide() {
        this.pageService.enableScroll();
        this._isLoadingAuthResponse.set(false);
    }
}
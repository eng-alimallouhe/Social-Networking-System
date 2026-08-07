import { inject, Injectable, signal } from "@angular/core";
import { PageService } from "../../../services/page.service";

@Injectable({ providedIn: 'root' })
export class LoadingOnboardingService {
    private pageService = inject(PageService);

    private _isLoadingOnboarding = signal(false);
    public isLoading = this._isLoadingOnboarding.asReadonly();

    show() {
        this.pageService.disableScroll();
        this._isLoadingOnboarding.set(true);
    }

    hide() {
        this.pageService.enableScroll();
        this._isLoadingOnboarding.set(false);
    }
}

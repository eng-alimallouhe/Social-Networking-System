import { inject, Injectable, signal } from "@angular/core";
import { PageService } from "../../../../shared/services/page.service";

@Injectable({ providedIn: 'root' })
export class LoadingSettingsService {
    private pageService = inject(PageService);

    private _isLoadingSettings = signal(false);
    public isLoadingSettings = this._isLoadingSettings.asReadonly();

    show() {
        this.pageService.disableScroll();
        this._isLoadingSettings.set(true);
    }

    hide() {
        this.pageService.enableScroll();
        this._isLoadingSettings.set(false);
    }
}
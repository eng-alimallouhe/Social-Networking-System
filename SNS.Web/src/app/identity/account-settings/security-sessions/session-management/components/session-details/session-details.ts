import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { LucideArrowLeft, LucideLaptop, LucideSmartphone, LucideTablet, LucideMonitor, LucideWifi, LucideLogOut } from '@lucide/angular';
import { ActivatedRoute, Router } from '@angular/router';
import { rxResource } from '@angular/core/rxjs-interop';
import { tap } from 'rxjs';
import { SessionManagementService } from '../../services/session-management.service';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { SessionDetailsDto } from '../../contracts/session-details.dto';
import { LocalDatePipe } from '../../../../../../shared/pipes/local-date.pipe';
import { getDeviceIcon } from '../shared/device-icon.helper';

@Component({
    selector: 'app-session-details',
    standalone: true,
    imports: [CommonModule, TranslatePipe, LucideArrowLeft, LucideLaptop, LucideSmartphone, LucideTablet, LucideMonitor, LucideWifi, LucideLogOut, LocalDatePipe],
    templateUrl: './session-details.html',
    styleUrls: ['./session-details.css']
})
export class SessionDetailsComponent {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private sessionService = inject(SessionManagementService);
    private globalLoader = inject(GlobalLoaderService);
    private translateService = inject(TranslateService);

    sessionId = computed(() => this.route.snapshot.paramMap.get('sessionId') || '');

    sessionsAndDevicesResource = rxResource({
        stream: () => {
            this.globalLoader.show();
            return this.sessionService.getSessionDetails(this.sessionId()).pipe(
                tap({
                    finalize: () => this.globalLoader.hide()
                })
            );
        }
    });

    session = computed(() => {
        const res = this.sessionsAndDevicesResource.value();
        if (res?.isSuccess && res.value) {
            return res.value;
        }
        return null;
    });

    isLoading = computed(() => this.sessionsAndDevicesResource.isLoading());
    
    error = computed(() => {
        const res = this.sessionsAndDevicesResource.value();
        if (res && !res.isSuccess) return 'Failed to load session details';
        if (this.sessionsAndDevicesResource.error()) return 'Failed to load session details';
        if (!this.isLoading() && res?.isSuccess && !this.session()) return 'Session not found';
        return null;
    });

    reload(): void {
        this.sessionsAndDevicesResource.reload();
    }

    isCurrentSession(session: SessionDetailsDto): boolean {
        return session.isViwerCurrentSession;
    }

    getIconName(os: string, deviceName: string): string {
        return getDeviceIcon(os, deviceName);
    }

    formatDuration(minutes: number): string {
        if (minutes < 60) {
            return this.translateService.instant('Identity.Security_Settings.Session_Management.Duration_Minutes', { minutes });
        }
        const hours = Math.floor(minutes / 60);
        const remainingMinutes = minutes % 60;
        return this.translateService.instant('Identity.Security_Settings.Session_Management.Duration_Hours', { hours, minutes: remainingMinutes });
    }

    goBack(): void {
        this.router.navigate(['/account-settings/sessions']);
    }

    triggerLogout(): void {
        // To be implemented or integrate with existing confirmState if needed
    }
}

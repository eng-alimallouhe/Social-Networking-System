import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideLogOut, LucideLaptop, LucideSmartphone, LucideTablet, LucideMapPin, LucideWifi, LucideShieldCheck, LucideClock, LucideHistory, LucideCalendar, LucideTrash2, LucideSettings, LucideInfo } from '@lucide/angular';
import { Router } from '@angular/router';
import { SessionManagementService } from '../../services/session-management.service';
import { TokenService } from '../../../../../shared/services/token.service';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { ConfirmStateService } from '../../../../../../shared/design-system/services/confirm-state.service';
import { ConfirmAction } from '../../../../../../shared/design-system/services/confirm-action.enum';
import { ActiveSessionDto } from '../../contracts/active-session.dto';
import { RegisteredDeviceDto } from '../../contracts/registered-device.dto';
import { AppConfirmDialog } from '../../../../../../shared/design-system/components/app-confirm-dialog/app-confirm-dialog';
import { LocalDatePipe } from '../../../../../../shared/pipes/local-date.pipe';
import { getDeviceIcon } from '../shared/device-icon.helper';
import { rxResource } from '@angular/core/rxjs-interop';
import { tap } from 'rxjs';

@Component({
    selector: 'app-sessions-and-devices',
    standalone: true,
    imports: [CommonModule, TranslatePipe, LucideLogOut, LucideLaptop, LucideSmartphone, LucideTablet, LucideMapPin, LucideWifi, LucideShieldCheck, LucideClock, LucideHistory, LucideCalendar, LucideTrash2, LucideSettings, LucideInfo, AppConfirmDialog, LocalDatePipe],
    templateUrl: './sessions-and-devices.html',
    styleUrls: ['./sessions-and-devices.css']
})
export class SessionsAndDevicesComponent {
    private sessionService = inject(SessionManagementService);
    private tokenService = inject(TokenService);
    private globalLoader = inject(GlobalLoaderService);
    private confirmState = inject(ConfirmStateService);
    private router = inject(Router);

    ConfirmAction = ConfirmAction;

    sessionsAndDevicesResource = rxResource({
        stream: () => {
            this.globalLoader.show();
            return this.sessionService.getUserActiveSessionsAndDevices().pipe(
                tap({
                    finalize: () => this.globalLoader.hide()
                })
            );
        }
    });

    activeSessions = computed(() => {
        const res = this.sessionsAndDevicesResource.value();
        return res?.isSuccess && res.value ? res.value.activeSessions : [];
    });

    registeredDevices = computed(() => {
        const res = this.sessionsAndDevicesResource.value();
        return res?.isSuccess && res.value ? res.value.registeredDevices : [];
    });

    recentSessions = computed(() => this.activeSessions().slice(0, 5));
    recentDevices = computed(() => this.registeredDevices().slice(0, 5));

    hasMoreSessions = computed(() => this.activeSessions().length > 5);
    hasMoreDevices = computed(() => this.registeredDevices().length > 5);

    error = computed(() => {
        const res = this.sessionsAndDevicesResource.value();
        if (res && !res.isSuccess) return 'Failed to load sessions and devices';
        if (this.sessionsAndDevicesResource.error()) return 'Failed to load sessions and devices';
        return null;
    });

    // Dialog state
    showLogoutConfirm = signal<boolean>(false);
    showLogoutAllConfirm = signal<boolean>(false);
    selectedSessionId = signal<string | null>(null);

    constructor() {
        effect(() => {
            const action = this.confirmState.consume();
            if (action === ConfirmAction.Revoke && this.showLogoutConfirm()) {
                this.executeLogoutSession();
            } else if (action === ConfirmAction.Revoke && this.showLogoutAllConfirm()) {
                this.executeLogoutAllOther();
            }
        });
    }

    reload(): void {
        this.sessionsAndDevicesResource.reload();
    }

    isCurrentSession(session: ActiveSessionDto): boolean {
        const currentSessionId = this.tokenService.getClaim('sid');
        return currentSessionId !== null && currentSessionId === session.sessionId;
    }

    promptLogoutSession(sessionId: string): void {
        this.selectedSessionId.set(sessionId);
        this.showLogoutConfirm.set(true);
    }

    cancelLogoutSession(): void {
        this.showLogoutConfirm.set(false);
        this.selectedSessionId.set(null);
    }

    promptLogoutAllOther(): void {
        this.showLogoutAllConfirm.set(true);
    }

    cancelLogoutAllOther(): void {
        this.showLogoutAllConfirm.set(false);
    }

    private executeLogoutSession(): void {
        const sessionId = this.selectedSessionId();
        if (!sessionId) return;

        this.showLogoutConfirm.set(false);
        this.globalLoader.show();
        this.sessionService.logoutFromSession({ sessionId }).subscribe({
            next: (result) => {
                this.globalLoader.hide();
                if (result.isSuccess) {
                    this.reload();
                }
            },
            error: () => {
                this.globalLoader.hide();
            }
        });
    }

    private executeLogoutAllOther(): void {
        this.showLogoutAllConfirm.set(false);
        this.globalLoader.show();
        this.sessionService.logoutFromOtherDevices().subscribe({
            next: (result) => {
                this.globalLoader.hide();
                if (result.isSuccess) {
                    this.reload();
                }
            },
            error: () => {
                this.globalLoader.hide();
            }
        });
    }

    getIconName(os: string, deviceName: string): string {
        return getDeviceIcon(os, deviceName);
    }

    getGoogleMapsUrl(location: string): string {
        return `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(location)}`;
    }

    viewSessionDetails(sessionId: string): void {
        this.router.navigate(['/account-settings/sessions', sessionId]);
    }

    viewAllSessions(): void {
        const userId = this.tokenService.getUserId();
        this.router.navigate(['/account-settings/sessions/all-sessions'], { queryParams: { userId } });
    }

    viewAllDevices(): void {
        this.router.navigate(['/account-settings/sessions/all-devices']);
    }
}

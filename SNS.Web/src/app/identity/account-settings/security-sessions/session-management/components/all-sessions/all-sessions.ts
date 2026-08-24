import { Component, effect, inject, computed, signal } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideLogOut, LucideLaptop, LucideSmartphone, LucideTablet, LucideMapPin, LucideArrowLeft, LucideClock } from '@lucide/angular';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { finalize, of } from 'rxjs';
import { SessionManagementService } from '../../services/session-management.service';
import { TokenService } from '../../../../../shared/services/token.service';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { ConfirmStateService } from '../../../../../../shared/design-system/services/confirm-state.service';
import { ConfirmAction } from '../../../../../../shared/design-system/services/confirm-action.enum';
import { SessionSummaryDto } from '../../contracts/session-summary.dto';
import { AppConfirmDialog } from '../../../../../../shared/design-system/components/app-confirm-dialog/app-confirm-dialog';
import { LocalDatePipe } from '../../../../../../shared/pipes/local-date.pipe';
import { getDeviceIcon } from '../shared/device-icon.helper';
import { AppPagination } from '../../../../../../shared/design-system/components/app-pagination/app-pagination';

@Component({
    selector: 'app-all-sessions',
    standalone: true,
    imports: [CommonModule, TranslatePipe, LucideLogOut, LucideLaptop, LucideSmartphone, LucideTablet, LucideMapPin, LucideArrowLeft, LucideClock, AppConfirmDialog, LocalDatePipe, AppPagination],
    templateUrl: './all-sessions.html',
    styleUrls: ['./all-sessions.css']
})
export class AllSessionsComponent {
    private sessionService = inject(SessionManagementService);
    private tokenService = inject(TokenService);
    private globalLoader = inject(GlobalLoaderService);
    private confirmState = inject(ConfirmStateService);
    private router = inject(Router);
    private route = inject(ActivatedRoute);
    private loaction = inject(Location);

    ConfirmAction = ConfirmAction;

    private queryParams = toSignal(this.route.queryParams, { initialValue: {} as Params });

    targetUserId = computed(() => {
        const queryId = this.queryParams()['userId'];
        if (queryId) return queryId;
        return this.tokenService.getClaim('uid') || this.tokenService.getClaim('http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier') || '';
    });

    currentPage = signal<number>(1);
    pageSize = signal<number>(10);
    justActiveSessions = signal<boolean>(false);

    sessionsResource = rxResource({
        params: () => ({
            userId: this.targetUserId(),
            justActive: this.justActiveSessions(),
            page: this.currentPage(),
            size: this.pageSize()
        }),
        stream: ({ params }) => {
            this.globalLoader.show();
            if (!params.userId) {
                this.globalLoader.hide();   
                return of({ isSuccess: false, error: { message: 'Failed to identify user' } } as any);
            }
            return this.sessionService.getUserSessions(params.userId, params.justActive, params.page, params.size)
                        .pipe(finalize(() => this.globalLoader.hide()));
        }
    });

    sessions = computed(() => {
        const result = this.sessionsResource.value() as any;
        if (result?.isSuccess && result.value) {
            return result.value.items;
        }
        return [];
    });

    totalPages = computed(() => {
        const result = this.sessionsResource.value() as any;
        if (result?.isSuccess && result.value) {
            return result.value.totalPages;
        }
        return 1;
    });

    isLoading = computed(() => this.sessionsResource.isLoading());

    error = computed(() => {
        if (this.sessionsResource.error()) {
            return 'Failed to load sessions';
        }
        const result = this.sessionsResource.value() as any;
        if (result && !result.isSuccess) {
            return result.error?.message || 'Failed to load sessions';
        }
        return null;
    });

    // Dialog state
    showLogoutConfirm = signal<boolean>(false);
    selectedSessionId = signal<string | null>(null);

    constructor() {
        effect(() => {
            const action = this.confirmState.consume();
            if (action === ConfirmAction.Revoke && this.showLogoutConfirm()) {
                this.executeLogoutSession();
            }
        });
    }

    onPageChange(page: number): void {
        this.currentPage.set(page);
    }

    toggleActiveFilter(event: Event): void {
        const input = event.target as HTMLInputElement;
        this.justActiveSessions.set(input.checked);
        this.currentPage.set(1);
    }

    isCurrentSession(session: SessionSummaryDto): boolean {
        const targetId = this.targetUserId();
        const currentUserId = this.tokenService.getClaim('uid') || this.tokenService.getClaim('http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier') || '';

        if (targetId !== currentUserId) {
            return false;
        }

        const currentSessionId = this.tokenService.getClaim('sid');
        return currentSessionId !== null && currentSessionId === session.id;
    }

    promptLogoutSession(sessionId: string): void {
        this.selectedSessionId.set(sessionId);
        this.showLogoutConfirm.set(true);
    }

    cancelLogoutSession(): void {
        this.showLogoutConfirm.set(false);
        this.selectedSessionId.set(null);
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
                    this.sessionsResource.reload();
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
        const queryParams = this.queryParams()['userId'] ? { userId: this.queryParams()['userId'] } : {};
        this.router.navigate(['/account-settings/security-settings/sessions/session', sessionId], { queryParams });
    }

    goBack(): void {
        const queryParams = this.queryParams()['userId'] ? { userId: this.queryParams()['userId'] } : {};
        this.loaction.back();
    }
}

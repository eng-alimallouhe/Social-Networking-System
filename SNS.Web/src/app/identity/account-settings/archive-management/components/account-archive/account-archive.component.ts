import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Params, Router, RouterModule } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ArchiveManagementService } from '../../services/archive-management.service';
import { ArchiveMessageBuilderService } from '../../services/archive-message-builder.service';
import { UserArchiveSummaryDto } from '../../contracts/archive-management.models';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { AuthenticationService } from '../../../../shared/services/authentication.service';
import { GlobalLoaderService } from '../../../../../shared/Loading/services/global-loader.service';
import { finalize, map } from 'rxjs';
import { CircleLoader } from "../../../../../shared/Loading/components/circle-loader/circle-loader";
import { AppPagination } from "../../../../../shared/design-system/components/app-pagination/app-pagination";
import { LucideArrowLeft } from '@lucide/angular';

@Component({
    selector: 'app-account-archive',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        TranslatePipe,
        CircleLoader,
        AppPagination,
        LucideArrowLeft
    ],
    templateUrl: './account-archive.component.html',
    styleUrls: ['./account-archive.component.css']
})
export class AccountArchiveComponent {

    private archiveService = inject(ArchiveManagementService);
    public messageBuilder = inject(ArchiveMessageBuilderService);
    public translate = inject(TranslateService);
    private route = inject(ActivatedRoute);
    private authenticationService = inject(AuthenticationService);
    private location = inject(Location);

    public currentPage = signal(1);
    public pageSize = signal(1);

    private queryParams = toSignal(
        this.route.queryParams,
        { initialValue: {} as Params }
    );

    targetUserId = computed(() => {
        const queryId = this.queryParams()['tuid'];

        if (queryId)
            return queryId;

        return this.authenticationService.getUserId()
            || this.authenticationService.getClaim(
                'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
            )
            || '';
    });

    accountResource = rxResource({
        params: () => ({
            page: this.currentPage(),
            size: this.pageSize(),
            targetUserId: this.targetUserId()
        }),

        stream: ({ params }) =>
            this.archiveService.getUserArchive({
                currentPage: params.page,
                pageSize: params.size,
                targetUserId: params.targetUserId
            })
    });

    readonly page = computed(() =>
        this.accountResource.value()?.value
    );

    readonly items = computed(() =>
        this.page()?.items ?? []
    );

    readonly totalPages = computed(() =>
        this.page()?.totalPages ?? 0
    );

    readonly totalItems = computed(() =>
        this.page()?.totalCount ?? 0
    );

    public loadPage(page: number): void {
        this.currentPage.set(page);
    }

    public getActionClass(type: string): string {
        const t = type.toLowerCase();

        if (t.includes('created') || t.includes('enabled'))
            return 'action-badge-success';

        if (t.includes('deleted') ||
            t.includes('banned') ||
            t.includes('suspended'))
            return 'action-badge-danger';

        return 'action-badge-primary';
    }

    public backToParent(): void {
        this.location.back();
    }
}
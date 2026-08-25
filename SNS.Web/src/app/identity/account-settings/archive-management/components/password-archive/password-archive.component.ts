import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Params, RouterModule } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ArchiveManagementService } from '../../services/archive-management.service';
import { UserPasswordArchiveSummaryDto } from '../../contracts/archive-management.models';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { TokenService } from '../../../../shared/services/token.service';
import { CircleLoader } from '../../../../../shared/Loading/components/circle-loader/circle-loader';
import { AppPagination } from '../../../../../shared/design-system/components/app-pagination/app-pagination';
import { LucideArrowLeft } from '@lucide/angular';

@Component({
    selector: 'app-password-archive',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        TranslatePipe,
        CircleLoader,
        AppPagination,
        LucideArrowLeft
    ],
    templateUrl: './password-archive.component.html',
    styleUrls: ['./password-archive.component.css']
})
export class PasswordArchiveComponent {

    private archiveService = inject(ArchiveManagementService);
    public translate = inject(TranslateService);

    private tokenService = inject(TokenService);
    private route = inject(ActivatedRoute);
    private location = inject(Location);

    public currentPage = signal(1);
    public pageSize = signal(10);

    private queryParams = toSignal(
        this.route.queryParams,
        { initialValue: {} as Params }
    );

    targetUserId = computed(() => {
        const queryId = this.queryParams()['tuid'];

        if (queryId)
            return queryId;

        return this.tokenService.getUserId()
            || this.tokenService.getClaim(
                'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
            )
            || '';
    });

    passwordResource = rxResource({
        params: () => ({
            page: this.currentPage(),
            size: this.pageSize(),
            targetUserId: this.targetUserId()
        }),

        stream: ({ params }) =>
            this.archiveService.getUserPasswordArchive({
                currentPage: params.page,
                pageSize: params.size,
                targetUserId: params.targetUserId
            })
    });

    readonly page = computed(() =>
        this.passwordResource.value()?.value
    );

    readonly items = computed<UserPasswordArchiveSummaryDto[]>(() =>
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

    public backToParent(): void {
        this.location.back();
    }
}
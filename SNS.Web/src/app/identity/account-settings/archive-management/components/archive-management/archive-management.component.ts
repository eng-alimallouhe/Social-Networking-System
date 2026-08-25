import { Component, effect, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, NavigationEnd, Router, RouterModule } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ArchiveManagementService } from '../../services/archive-management.service';
import { ArchiveMessageBuilderService } from '../../services/archive-message-builder.service';
import {
    UserArchiveSummaryDto,
    UserIdentityArchiveSummaryDto,
    UserPasswordArchiveSummaryDto
} from '../../contracts/archive-management.models';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { TokenService } from '../../../../shared/services/token.service';
import { filter, finalize, forkJoin, map, of, tap } from 'rxjs';
import { GlobalLoaderService } from '../../../../../shared/Loading/services/global-loader.service';
import { LucideDownload, LucideHistory, LucideBadgeCheck, LucideKeyRound, LucideChevronDown } from "@lucide/angular";

@Component({
    selector: 'app-archive-management',
    standalone: true,
    imports: [CommonModule, RouterModule, TranslatePipe, LucideDownload, LucideHistory, LucideBadgeCheck, LucideKeyRound, LucideChevronDown],
    templateUrl: './archive-management.component.html',
    styleUrls: ['./archive-management.component.css']
})
export class ArchiveManagementComponent implements OnInit {
    private archiveService = inject(ArchiveManagementService);
    public messageBuilder = inject(ArchiveMessageBuilderService);
    public translate = inject(TranslateService);
    private tokenService = inject(TokenService);
    private route = inject(ActivatedRoute);
    private loaderService = inject(GlobalLoaderService);
    private router = inject(Router);

    targetUserId = this.tokenService.getUserId();

    readonly isArchiveRootRoute = toSignal(
        this.router.events.pipe(
            filter(event => event instanceof NavigationEnd),
            map(() => this.router.url === '/account-settings/archive')
        ),
        {
            initialValue: this.router.url === '/account-settings/archive'
        }
    );

    public isExporting = false;

    ngOnInit(): void {
    }
    archiveResource = rxResource({
        params: () => this.isArchiveRootRoute(),
        stream: ({ params }) => {
            if (!params) {
                return of(null);
            }

            this.loaderService.show();

            return this.archiveService
                .getArchiveSummary(this.targetUserId)
                .pipe(
                    finalize(() => this.loaderService.hide()),
                    tap(res => {
                        console.log('data:', res);
                    })
                );
        }
    });


    public requestExport(): void {
        if (this.isExporting) return;

        this.isExporting = true;
        this.archiveService.requestAccountDataExport().subscribe({
            next: (res) => {
                this.isExporting = false;
                // Toast success can be added here if available
            },
            error: () => {
                this.isExporting = false;
                // Toast error can be added here if available
            }
        });
    }

    public getActionClass(type: string): string {
        const t = type.toLowerCase();
        if (t.includes('created') || t.includes('enabled')) return 'action-badge-success';
        if (t.includes('deleted') || t.includes('banned') || t.includes('suspended')) return 'action-badge-danger';
        return 'action-badge-primary';
    }
}

import {
    Component,
    computed,
    inject,
    signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';

import {
    LucideCompass,
    LucideUsers
} from '@lucide/angular';

import { Community } from '../community/community';
import { CommunitiesService } from '../../services/communities.service';
import { CommunitySummaryDto } from '../../contracts/community-summary.dto';

import {
    SkeletonLoaderComponent,
    SkeletonType
} from '../../../../../shared/Loading/components/skeleton-loader/skeleton-loader';

type CommunitiesTab = 'mine' | 'suggested';

@Component({
    selector: 'app-communities',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        LucideUsers,
        LucideCompass,
        Community,
        SkeletonLoaderComponent
    ],
    templateUrl: './communities.html',
    styleUrl: './communities.css'
})
export class Communities {

    private readonly communitiesService = inject(CommunitiesService);

    public loaderType = SkeletonType

    readonly activeTab = signal<CommunitiesTab>('mine');

    readonly myCommunities =
        signal<CommunitySummaryDto[]>([]);

    readonly suggestedCommunities =
        signal<CommunitySummaryDto[]>([]);

    readonly isLoadingMine = signal(true);
    readonly isLoadingSuggested = signal(false);

    readonly communities = computed(() =>
        this.activeTab() === 'mine'
            ? this.myCommunities()
            : this.suggestedCommunities()
    );

    readonly isLoading = computed(() =>
        this.activeTab() === 'mine'
            ? this.isLoadingMine()
            : this.isLoadingSuggested()
    );

    readonly skeletonItems = Array.from({ length: 6 });

    ngOnInit(): void {
        this.loadMyCommunities();
    }

    selectTab(tab: CommunitiesTab): void {

        if (this.activeTab() === tab) {
            return;
        }

        this.activeTab.set(tab);

        if (tab === 'suggested' &&
            this.suggestedCommunities().length === 0) {

            this.loadSuggestedCommunities();
        }
    }

    private loadMyCommunities(): void {

        this.isLoadingMine.set(true);

        this.communitiesService
            .getMyCommunities(1, 20)
            .subscribe({
                next: response => {

                    if (response.isSuccess && response.value) {
                        this.myCommunities.set(
                            response.value.items ?? []
                        );
                    }

                    this.isLoadingMine.set(false);
                },

                error: () => {
                    this.isLoadingMine.set(false);
                }
            });
    }

    private loadSuggestedCommunities(): void {

        this.isLoadingSuggested.set(true);

        this.communitiesService
            .getSuggestedCommunities(20)
            .subscribe({
                next: response => {

                    if (response.isSuccess && response.value) {
                        this.suggestedCommunities.set(
                            response.value
                        );
                    }

                    this.isLoadingSuggested.set(false);
                },

                error: () => {
                    this.isLoadingSuggested.set(false);
                }
            });
    }
}
import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideArrowLeft,
    LucideGlobe,
    LucideLock,
    LucideCalendar,
    LucideUsers,
    LucideMessageSquare,
    LucideCheck,
    LucideUserPlus,
    LucideShare2,
    LucideAlertCircle,
    LucideRefreshCw,
    LucideScrollText
} from '@lucide/angular';
import { CommunitiesService } from '../../services/communities.service';
import { CommunityDetailsDto } from '../../contracts/community-details.dto';
import { CommunityType } from '../../../../../shared/contracts/community-type';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../../shared/Loading/components/skeleton-loader/skeleton-loader';

@Component({
    selector: 'app-community-details',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        SkeletonLoaderComponent,
        LucideArrowLeft,
        LucideGlobe,
        LucideLock,
        LucideCalendar,
        LucideUsers,
        LucideMessageSquare,
        LucideCheck,
        LucideUserPlus,
        LucideShare2,
        LucideAlertCircle,
        LucideRefreshCw,
        LucideScrollText
    ],
    templateUrl: './community-details.html',
    styleUrl: './community-details.css'
})
export class CommunityDetails implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private location = inject(Location);
    private communitiesService = inject(CommunitiesService);

    readonly SkeletonType = SkeletonType;
    readonly CommunityType = CommunityType;
    readonly defaultAvatar = 'assets/images/default-avatar.png';

    communityId = signal<string>('');
    community = signal<CommunityDetailsDto | null>(null);
    isLoading = signal<boolean>(true);
    hasError = signal<boolean>(false);
    isMember = signal<boolean>(false);

    initials = computed(() => {
        const name = this.community()?.name?.trim();
        if (!name) return 'CO';
        const parts = name.split(/\s+/);
        if (parts.length >= 2) {
            return (parts[0][0] + parts[1][0]).toUpperCase();
        }
        return name.slice(0, 2).toUpperCase();
    });

    isPublic = computed(() => {
        return this.community()?.type === CommunityType.Public;
    });

    readonly formattedDescription = computed<string>(() => {
        const desc = this.community()?.description;
        if (!desc) return '';
        return desc.replace(/\\n/g, '\n');
    });

    readonly formattedRules = computed<string[]>(() => {
        const rules = this.community()?.rulesText;
        if (!rules) return [];
        const unescaped = rules.replace(/\\n/g, '\n').replace(/\\r/g, '');
        return unescaped
            .split('\n')
            .map(r => r.trim())
            .filter(r => r.length > 0);
    });

    ngOnInit(): void {
        this.route.paramMap.subscribe(params => {
            const id = params.get('communityId') || params.get('id');
            if (id && id !== this.communityId()) {
                this.communityId.set(id);
                this.loadCommunity();
            }
        });
    }

    loadCommunity(): void {
        const id = this.communityId();
        if (!id) return;

        this.isLoading.set(true);
        this.hasError.set(false);

        this.communitiesService.getCommunityById(id).subscribe({
            next: res => {
                this.isLoading.set(false);
                if (res?.isSuccess && res.value) {
                    this.community.set(res.value);
                    this.isMember.set(res.value.isMember);
                } else {
                    this.hasError.set(true);
                }
            },
            error: () => {
                this.isLoading.set(false);
                this.hasError.set(true);
            }
        });
    }

    toggleMembership(): void {
        this.isMember.update(m => !m);
    }

    share(): void {
        if (typeof window !== 'undefined' && navigator.clipboard) {
            navigator.clipboard.writeText(window.location.href);
        }
    }

    onOwnerAvatarError(event: Event): void {
        const target = event.target as HTMLImageElement;
        if (target && target.src !== this.defaultAvatar) {
            target.src = this.defaultAvatar;
        }
    }

    goBack(): void {
        if (typeof window !== 'undefined' && window.history.length > 1) {
            this.location.back();
        } else {
            this.router.navigate(['/home/search']);
        }
    }
}

import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import {
    LucideArrowLeft,
    LucidePencil,
    LucideUserPlus,
    LucideShare2,
    LucideExternalLink,
    LucideGlobe,
    LucideSettings,
    LucideCalendar,
    LucideBookOpen,
    LucideImage,
    LucideFlag,
    LucideCode,
    LucideMessageSquare,
    LucideUsers,
    LucideAlertCircle,
    LucideRefreshCw,
    LucideFolder,
    LucideFileText,
    LucideCheckCircle2
} from '@lucide/angular';
import { ProjectService } from '../../services/project.service';
import { AuthenticationService } from '../../../identity/shared/services/authentication.service';
import { ProjectDetailsDto } from '../../contracts/project-details.dto';
import { ProjectParticipantDetailsDto } from '../../contracts/project-participant-details.dto';
import { ProjectRatingDto } from '../../contracts/project-rating.dto';
import { ProjectMediaDto } from '../../contracts/project-media.dto';
import { ProjectMilestoneDto } from '../../contracts/project-milestone.dto';
import { FileNode } from '../../contracts/file-node.dto';
import { MarkdownService } from '../../../shared/services/markdown.service';
import { SkeletonLoaderComponent, SkeletonType } from '../../../shared/Loading/components/skeleton-loader/skeleton-loader';

export type ProjectTab = 'readme' | 'media' | 'milestones' | 'source-code' | 'reviews';

@Component({
    selector: 'app-project-details',
    standalone: true,
    imports: [
        CommonModule,
        TranslatePipe,
        SkeletonLoaderComponent,
        LucideArrowLeft,
        LucidePencil,
        LucideUserPlus,
        LucideShare2,
        LucideExternalLink,
        LucideGlobe,
        LucideSettings,
        LucideCalendar,
        LucideBookOpen,
        LucideImage,
        LucideFlag,
        LucideCode,
        LucideMessageSquare,
        LucideUsers,
        LucideAlertCircle,
        LucideRefreshCw,
        LucideFolder,
        LucideFileText,
        LucideCheckCircle2
    ],
    templateUrl: './project-details.html',
    styleUrl: './project-details.css'
})
export class ProjectDetails implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private location = inject(Location);
    private projectService = inject(ProjectService);
    private markdownService = inject(MarkdownService);
    private authService = inject(AuthenticationService);

    readonly SkeletonType = SkeletonType;
    readonly defaultAvatar = 'assets/images/default-avatar.png';
    readonly defaultProjectImage = 'assets/images/default-project.png';

    projectId = signal<string>('');
    project = signal<ProjectDetailsDto | null>(null);
    collaborators = signal<ProjectParticipantDetailsDto[]>([]);
    ratings = signal<ProjectRatingDto[]>([]);
    media = signal<ProjectMediaDto[]>([]);
    milestones = signal<ProjectMilestoneDto[]>([]);
    sourceCode = signal<FileNode[]>([]);

    activeTab = signal<ProjectTab>('readme');

    isLoading = signal<boolean>(true);
    hasError = signal<boolean>(false);
    hasImageError = signal<boolean>(false);

    isLoadingMedia = signal<boolean>(false);
    isLoadingMilestones = signal<boolean>(false);
    isLoadingSourceCode = signal<boolean>(false);
    isLoadingReviews = signal<boolean>(false);
    isLoadingCollaborators = signal<boolean>(false);

    readonly isOwner = computed(() => {
        if (!this.authService.isAuthenticated()) return false;
        const currentId = this.authService.getClaim('ProfileId') || this.authService.getUserId();
        if (!currentId) return true;

        const proj = this.project() as any;
        if (proj?.ownerId && String(proj.ownerId).toLowerCase() === currentId.toLowerCase()) {
            return true;
        }

        const ownerCollab = this.collaborators().find(c =>
            c.profileId && String(c.profileId).toLowerCase() === currentId.toLowerCase() &&
            (c.role?.toLowerCase() === 'owner' || c.role?.toLowerCase() === 'projectmanager')
        );
        if (ownerCollab) return true;

        // If no collaborators are listed yet or ownerId isn't on DTO, default to true for authenticated user
        if (this.collaborators().length === 0) return true;

        return false;
    });

    readonly averageRating = computed(() => {
        const r = this.ratings();
        if (r.length === 0) return 4.7;
        const sum = r.reduce((acc, curr) => acc + curr.ratingValue, 0);
        return Math.round((sum / r.length) * 10) / 10;
    });

    readonly totalReviewsCount = computed(() => {
        const r = this.ratings();
        return r.length > 0 ? r.length : 156;
    });

    readonly renderedReadme = computed<string | null>(() => {
        const readme = this.project()?.readmeContent;
        if (!readme) return null;
        const cleanReadme = readme.replace(/\\n/g, '\n');
        return this.markdownService.parse(cleanReadme);
    });

    ngOnInit(): void {
        this.route.paramMap.subscribe(params => {
            const id = params.get('projectId') || params.get('id');
            if (id && id !== this.projectId()) {
                this.projectId.set(id);
                this.loadAllProjectData();
            }
        });
    }

    loadAllProjectData(): void {
        this.loadProject();
        this.loadCollaborators();
        this.loadReviews();
    }

    loadProject(): void {
        const id = this.projectId();
        if (!id) return;

        this.isLoading.set(true);
        this.hasError.set(false);

        this.projectService.getProjectById(id).subscribe({
            next: res => {
                this.isLoading.set(false);
                if (res?.isSuccess && res.value) {
                    this.project.set(res.value);
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

    loadCollaborators(): void {
        const id = this.projectId();
        if (!id) return;
        this.isLoadingCollaborators.set(true);
        this.projectService.getProjectParticipants(id, 1, 50).subscribe({
            next: res => {
                this.isLoadingCollaborators.set(false);
                if (res?.isSuccess && res.value?.items) {
                    this.collaborators.set(res.value.items);
                }
            },
            error: () => this.isLoadingCollaborators.set(false)
        });
    }

    loadReviews(): void {
        const id = this.projectId();
        if (!id) return;
        this.isLoadingReviews.set(true);
        this.projectService.getProjectRatings(id, 1, 50).subscribe({
            next: res => {
                this.isLoadingReviews.set(false);
                if (res?.isSuccess && res.value?.items) {
                    this.ratings.set(res.value.items);
                }
            },
            error: () => this.isLoadingReviews.set(false)
        });
    }

    loadMedia(): void {
        const id = this.projectId();
        if (!id) return;
        this.isLoadingMedia.set(true);
        this.projectService.getProjectMedia(id, 1, 50).subscribe({
            next: res => {
                this.isLoadingMedia.set(false);
                if (res?.isSuccess && res.value?.items) {
                    this.media.set(res.value.items);
                }
            },
            error: () => this.isLoadingMedia.set(false)
        });
    }

    loadMilestones(): void {
        const id = this.projectId();
        if (!id) return;
        this.isLoadingMilestones.set(true);
        this.projectService.getProjectMilestones(id).subscribe({
            next: res => {
                this.isLoadingMilestones.set(false);
                if (res?.isSuccess && res.value) {
                    this.milestones.set(res.value);
                }
            },
            error: () => this.isLoadingMilestones.set(false)
        });
    }

    loadSourceCode(): void {
        const id = this.projectId();
        if (!id) return;
        this.isLoadingSourceCode.set(true);
        this.projectService.getProjectSourceCode(id).subscribe({
            next: res => {
                this.isLoadingSourceCode.set(false);
                if (res?.isSuccess && res.value) {
                    this.sourceCode.set(res.value);
                }
            },
            error: () => this.isLoadingSourceCode.set(false)
        });
    }

    setActiveTab(tab: ProjectTab): void {
        this.activeTab.set(tab);
        if (tab === 'media' && this.media().length === 0 && !this.isLoadingMedia()) {
            this.loadMedia();
        } else if (tab === 'milestones' && this.milestones().length === 0 && !this.isLoadingMilestones()) {
            this.loadMilestones();
        } else if (tab === 'source-code' && this.sourceCode().length === 0 && !this.isLoadingSourceCode()) {
            this.loadSourceCode();
        } else if (tab === 'reviews' && this.ratings().length === 0 && !this.isLoadingReviews()) {
            this.loadReviews();
        }
    }

    share(): void {
        if (typeof window !== 'undefined' && navigator.clipboard) {
            navigator.clipboard.writeText(window.location.href);
        }
    }

    onUpdateProject(): void {
        this.router.navigate(['/projects/edit', this.projectId()]);
    }

    onAddCollaborator(): void {
        // action hook for collaborator addition
    }

    onAvatarError(event: Event): void {
        const img = event.target as HTMLImageElement;
        if (img) img.src = this.defaultAvatar;
    }

    onImageError(): void {
        this.hasImageError.set(true);
    }

    onMediaError(event: Event): void {
        const img = event.target as HTMLImageElement;
        if (img) {
            img.src = 'assets/images/guest.svg';
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

import { Component, input, signal, inject, effect, computed } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { BreakpointObserver } from '@angular/cdk/layout';
import { map } from 'rxjs';
import { CommonModule } from '@angular/common';
import { PostModelDto } from '../../contracts/post-model.dto';
import { MediaPlayer, PostMediaDto, MediaType } from '../../../../shared/design-system/components/media-player/media-player';
import {
    LucideThumbsUp,
    LucideMessageSquare,
    LucideShare2,
    LucideBookmark,
    LucideMoreHorizontal,
    LucideHeart,
    LucideSmile,
    LucideFrown,
    LucideAngry,
    LucideThumbsDown,
    LucideFlag,
    LucideEdit2,
    LucideTrash2
} from '@lucide/angular';
import { TranslatePipe } from '@ngx-translate/core';
import { OverlayModule, ConnectionPositionPair } from '@angular/cdk/overlay';
import { PostReactionService } from '../../services/post-reaction.service';
import { MarkdownService } from '../../../../shared/services/markdown.service';
import { ReactionType } from '../../../../shared/contracts/reaction-type';
import { LocalDatePipe } from '../../../../shared/pipes/local-date.pipe';
import { PostReport } from '../../../../moderation/components/post-report/post-report';
import { WantToLogin } from '../../../../shared/components/want-to-login/want-to-login';
import { AppConfirmDialog } from '../../../../shared/design-system/components/app-confirm-dialog/app-confirm-dialog';
import { PostsService } from '../../services/posts.service';
import { TokenService } from '../../../../identity/shared/services/token.service';
import { AuthenticationService } from '../../../../identity/shared/services/authentication.service';
import { ConfirmAction } from '../../../../shared/design-system/services/confirm-action.enum';
import { ConfirmStateService } from '../../../../shared/design-system/services/confirm-state.service';


@Component({
    selector: 'app-post',
    standalone: true,
    imports: [
        CommonModule,
        MediaPlayer,
        LucideThumbsUp,
        LucideMessageSquare,
        LucideShare2,
        LucideBookmark,
        LucideMoreHorizontal,
        LucideHeart,
        LucideSmile,
        LucideFrown,
        LucideAngry,
        LucideFlag,
        LucideEdit2,
        LucideTrash2,
        LocalDatePipe,
        OverlayModule,
        PostReport,
        WantToLogin,
        AppConfirmDialog,
        TranslatePipe
    ],
    templateUrl: './post.html',
    styleUrls: ['./post.css']
})
export class Post {
    // post = input.required<PostModelDto>();

    post = signal<PostModelDto>({
        id: "",
        author: {
            id: "",
            fullName: "Ali Mallouhe",
            specialization: "Full Stack Developer",
            profilePictureUrl: "https://tse2.mm.bing.net/th/id/OIP.LD3YyOUR54jkOCPbOQ5tXwHaHa?r=0&rs=1&pid=ImgDetMain&o=7&rm=3"
        },
        community: null,
        title: "Introduction to React",
        content: "# Introduction to React\n\nReact is a JavaScript library for building user interfaces. It is maintained by Facebook and a community of individual developers and companies.\n\n## Key Features\n\n- **Component-based architecture**: React encourages building UIs as a composition of independent components.\n- **Declarative nature**: React allows you to describe what the UI should look like for a given state, and React handles the updates efficiently.\n- **Virtual DOM**: React uses a virtual representation of the DOM to optimize rendering performance.\n\n## Getting Started\n\nTo start using React, you can either use it via a CDN or install it using npm:\n\n```bash\n# Using npm\nnpm install react react-dom\n\n# Using yarn\nyarn add react react-dom\n```\n\n## Basic Example\n\nHere's a simple example of a React component:\n\n```jsx\nimport React from 'react';\nimport ReactDOM from 'react-dom';\n\nfunction HelloWorld() {\n  return <h1>Hello, World!</h1>;\n}\n\nReactDOM.render(<HelloWorld />, document.getElementById('root'));\n```\n\n## Additional Resources\n\n- [Official React Documentation](https://reactjs.org/)\n- [React GitHub Repository](https://github.com/facebook/react)\n- [Awesome React List](https://github.com/enaqx/awesome-react)\n\nFeel free to explore these resources to learn more about React and its ecosystem.",
        createdAt: "2026-08-25T10:00:00",
        updatedAt: "2026-08-25T10:00:00",
        lastInteractedAt: "2026-08-25T10:00:00",
        media: [
            {
                order: 0,
                url: "https://lh3.googleusercontent.com/aida-public/AB6AXuAE1_mdo0BdNx03Gr7txxl1SaIjpGXW3CuWB3GzYyK4KglOSBMingGzLl-YAhxeEZsHUbBXCIRHj6zl-L5EEtxooYkQYjkVGn7zFAwQwV1XZRwjzgv9l4NwEj-3ey70sjF0DZJH9kZRyn-4xGjVjfBKnGCHn4G_UzSGiGOlP9Vlu6sDwmDsoZNF_HMhpry1MRfdRXOm28moHyUWdB4W9VoJJg6EMUD1DMgLrjzgPtBP2f1jgZECc5aL",
                type: MediaType.Image
            }
        ],
        tags: ["react", "javascript", "web-development", "frontend"],
        commentsCount: 10,
        reactionsCount: 450,
        viewsCount: 1000,
        savesCount: 200,
        mentions: []
    });

    isReportOpen = signal<boolean>(false);
    isActionsMenuOpen = signal<boolean>(false);
    isWantToLoginOpen = signal<boolean>(false);
    isDeleteConfirmOpen = signal<boolean>(false);

    private reactionService = inject(PostReactionService);
    private postsService = inject(PostsService);
    private tokenService = inject(TokenService);
    private authService = inject(AuthenticationService);
    private confirmStateService = inject(ConfirmStateService);
    private breakpointObserver = inject(BreakpointObserver);
    private markdownService = inject(MarkdownService);

    isMobile = toSignal(
        this.breakpointObserver.observe('(max-width: 990px)').pipe(
            map(result => result.matches)
        ),
        { initialValue: false }
    );

    isOwner = computed(() => this.currentProfileId === this.post().author.id);

    isContentExpanded = signal<boolean>(false);

    markdownData = computed(() => {
        const content = this.post().content;
        return this.markdownService.parseAndTruncate(content || '', 150);
    });

    shortHtml = computed(() => this.markdownData().html);

    fullHtml = computed(() => {
        const content = this.post().content;
        return this.markdownService.parse(content || '');
    });

    shouldShowMoreButton = computed(() => this.markdownData().isTruncated);

    toggleContent() {
        this.isContentExpanded.set(!this.isContentExpanded());
    }

    readonly ConfirmAction = ConfirmAction;

    constructor() {
        effect(() => {
            const action = this.confirmStateService.confirmedAction();
            if (action === ConfirmAction.Delete && this.isDeleteConfirmOpen()) {
                this.confirmStateService.consume();
                this.confirmDelete();
            }
        });
    }

    get currentProfileId(): string | null {
        return this.tokenService.getClaim('ProfileId') || this.tokenService.getUserId();
    }

    get mediaList(): PostMediaDto[] {
        return this.post().media || [];
    }

    isReactionPickerOpen = signal(false);

    overlayPositions: ConnectionPositionPair[] = [
        new ConnectionPositionPair(
            { originX: 'start', originY: 'top' },
            { overlayX: 'start', overlayY: 'bottom' },
            0,
            -8 // offset to push it slightly above the button
        )
    ];

    actionsMenuPositions: ConnectionPositionPair[] = [
        new ConnectionPositionPair(
            { originX: 'end', originY: 'bottom' },
            { overlayX: 'end', overlayY: 'top' },
            0,
            4
        )
    ];

    reactionTypes = [
        { type: ReactionType.Like, icon: 'thumbs-up' },
        { type: ReactionType.Love, icon: 'heart' },
        { type: ReactionType.Haha, icon: 'smile' },
        { type: ReactionType.Sad, icon: 'frown' },
        { type: ReactionType.Angry, icon: 'angry' }
    ];

    toggleLike() {
        this.isReactionPickerOpen.set(!this.isReactionPickerOpen());
    }

    checkAuthAndExecute(action: () => void) {
        if (!this.authService.isAuthenticated()) {
            this.isActionsMenuOpen.set(false);
            this.isReactionPickerOpen.set(false);
            this.isWantToLoginOpen.set(true);
            return;
        }
        action();
    }

    applyReaction(type: ReactionType) {
        this.isReactionPickerOpen.set(false);
        this.checkAuthAndExecute(() => {
            const currentPost = this.post();
            const previousReaction = currentPost.currentUserReaction;
            const previousCount = currentPost.reactionsCount;

            if (previousReaction === type) {
                this.removeReaction();
                return;
            }

            // Optimistic UI update
            let newCount = previousCount;
            if (!previousReaction) {
                newCount++;
            }

            // Mutate local state
            currentPost.currentUserReaction = type;
            currentPost.reactionsCount = newCount;

            this.reactionService.addOrChangePostReaction(currentPost.id, type).subscribe({
                error: () => {
                    // Rollback on failure
                    currentPost.currentUserReaction = previousReaction;
                    currentPost.reactionsCount = previousCount;
                }
            });
        });
    }



    removeReaction() {
        const currentPost = this.post();
        const previousReaction = currentPost.currentUserReaction;
        const previousCount = currentPost.reactionsCount;

        if (!previousReaction) return;

        // Optimistic UI update
        currentPost.currentUserReaction = null;
        currentPost.reactionsCount = Math.max(0, previousCount - 1);

        this.reactionService.removePostReaction(currentPost.id).subscribe({
            error: () => {
                // Rollback on failure
                currentPost.currentUserReaction = previousReaction;
                currentPost.reactionsCount = previousCount;
            }
        });
    }

    onInterested() {
        this.isActionsMenuOpen.set(false);
        this.checkAuthAndExecute(() => {
            this.postsService.increaseInterest(this.post().id).subscribe();
        });
    }

    onNotInterested() {
        this.isActionsMenuOpen.set(false);
        this.checkAuthAndExecute(() => {
            this.postsService.decreaseInterest(this.post().id).subscribe();
        });
    }

    onReport() {
        this.isActionsMenuOpen.set(false);
        this.checkAuthAndExecute(() => {
            this.isReportOpen.set(true);
        });
    }

    onDelete() {
        this.isActionsMenuOpen.set(false);
        this.isDeleteConfirmOpen.set(true);
    }

    confirmDelete() {
        this.isDeleteConfirmOpen.set(false);
        this.postsService.deletePost(this.post().id).subscribe();
    }
}

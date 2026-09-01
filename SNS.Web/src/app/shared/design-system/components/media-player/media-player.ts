import { Component, Input, Output, EventEmitter, signal, ViewChild, ElementRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
export enum MediaType {
    Image = 'Image',
    Video = 'Video'
}

export interface PostMediaDto {
    order: number;
    url: string;
    type: MediaType;
}
import { LucideX, LucideChevronRight, LucideChevronLeft, LucidePlay } from '@lucide/angular';

export type MediaPlayerMode = 'inline' | 'modal';

@Component({
    selector: 'app-media-player',
    standalone: true,
    imports: [CommonModule, LucideX, LucideChevronRight, LucideChevronLeft, LucidePlay],
    templateUrl: './media-player.html',
    styleUrls: ['./media-player.css']
})
export class MediaPlayer implements OnInit {
    /** List of media items to display, sorted by their order field */
    @Input({ required: true }) mediaList: PostMediaDto[] = [];

    /** Starting index (used mainly for modal mode) */
    @Input() initialIndex: number = 0;

    /**
     * Display mode:
     * - 'inline': renders directly inside the post card, compact, supports navigation
     * - 'modal': full-screen overlay with close button (original behavior)
     */
    @Input() mode: MediaPlayerMode = 'modal';

    /** Emitted when the modal close button is clicked (modal mode only) */
    @Output() close = new EventEmitter<void>();

    @ViewChild('videoElement') videoElement!: ElementRef<HTMLVideoElement>;

    readonly MediaType = MediaType;

    currentIndex = signal<number>(0);
    isPlaying = signal<boolean>(false);

    /** Sorted media list, respecting the order field */
    get sortedMedia(): PostMediaDto[] {
        return [...this.mediaList].sort((a, b) => a.order - b.order);
    }

    get currentMedia(): PostMediaDto {
        return this.sortedMedia[this.currentIndex()];
    }

    get hasMultiple(): boolean {
        return this.sortedMedia.length > 1;
    }

    get isInline(): boolean {
        return this.mode === 'inline';
    }

    ngOnInit(): void {
        this.currentIndex.set(this.initialIndex);
    }

    closeModal(): void {
        this.close.emit();
    }

    private pauseCurrentVideo(): void {
        const video = this.videoElement?.nativeElement;
        if (video && !video.paused) {
            video.pause();
        }
    }

    next(): void {
        if (this.currentIndex() < this.sortedMedia.length - 1) {
            this.pauseCurrentVideo();
            this.currentIndex.update(i => i + 1);
            this.isPlaying.set(false);
        }
    }

    prev(): void {
        if (this.currentIndex() > 0) {
            this.pauseCurrentVideo();
            this.currentIndex.update(i => i - 1);
            this.isPlaying.set(false);
        }
    }

    selectMedia(index: number): void {
        this.pauseCurrentVideo();
        this.currentIndex.set(index);
        this.isPlaying.set(false);
    }

    toggleVideoPlayback(): void {
        const video = this.videoElement?.nativeElement;
        if (!video) return;

        if (video.paused) {
            const playPromise = video.play();
            if (playPromise !== undefined) {
                playPromise.then(() => {
                    this.isPlaying.set(true);
                }).catch(error => {
                    if (error.name === 'AbortError') {
                        console.log('Video play was safely interrupted by navigation.');
                    }
                    this.isPlaying.set(false);
                });
            }
        } else {
            video.pause();
            this.isPlaying.set(false);
        }
    }
}
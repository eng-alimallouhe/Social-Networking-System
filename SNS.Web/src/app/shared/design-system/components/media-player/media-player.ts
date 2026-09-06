import { Component, Input, Output, EventEmitter, signal, ViewChild, ElementRef, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideX, LucideChevronRight, LucideChevronLeft, LucidePlay } from '@lucide/angular';
import { TranslatePipe } from '@ngx-translate/core';

export enum MediaType {
    Image = 'Image',
    Video = 'Video'
}

export interface PostMediaDto {
    order: number;
    url: string;
    type: MediaType;
}

@Component({
    selector: 'app-media-player',
    standalone: true,
    imports: [CommonModule, LucideX, LucideChevronRight, LucideChevronLeft, LucidePlay, TranslatePipe],
    templateUrl: './media-player.html',
    styleUrls: ['./media-player.css']
})
export class MediaPlayer implements OnInit {
    /** List of media items to display, sorted by their order field */
    @Input({ required: true }) mediaList: PostMediaDto[] = [];

    /** Starting index */
    @Input() initialIndex: number = 0;

    /** Emitted when the preview is closed */
    @Output() close = new EventEmitter<void>();

    @ViewChild('videoElement') videoElement?: ElementRef<HTMLVideoElement>;

    readonly MediaType = MediaType;

    currentIndex = signal<number>(0);
    isPlaying = signal<boolean>(false);
    isPreviewOpen = signal<boolean>(false);

    /** Sorted media list, respecting the order field */
    get sortedMedia(): PostMediaDto[] {
        return [...this.mediaList].sort((a, b) => a.order - b.order);
    }

    get currentMedia(): PostMediaDto | undefined {
        return this.sortedMedia[this.currentIndex()];
    }

    get hasMultiple(): boolean {
        return this.sortedMedia.length > 1;
    }

    ngOnInit(): void {
        this.currentIndex.set(this.initialIndex);
    }

    @HostListener('window:keydown.escape')
    onEscape(): void {
        if (this.isPreviewOpen()) {
            this.closePreview();
        }
    }

    openPreview(index: number = 0): void {
        this.pauseCurrentVideo();
        const safeIndex = Math.max(0, Math.min(index, this.sortedMedia.length - 1));
        this.currentIndex.set(safeIndex);
        this.isPlaying.set(false);
        this.isPreviewOpen.set(true);
    }

    closePreview(): void {
        this.pauseCurrentVideo();
        this.isPlaying.set(false);
        this.isPreviewOpen.set(false);
        this.close.emit();
    }

    closeModal(): void {
        this.closePreview();
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
        if (index >= 0 && index < this.sortedMedia.length) {
            this.pauseCurrentVideo();
            this.currentIndex.set(index);
            this.isPlaying.set(false);
        }
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
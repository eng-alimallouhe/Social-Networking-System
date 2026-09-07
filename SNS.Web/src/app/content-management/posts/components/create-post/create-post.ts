import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { 
  LucideImage, 
  LucideUpload, 
  LucideX, 
  LucideEye, 
  LucideEdit3, 
  LucideSend, 
  LucideArrowLeft,
  LucideAlertCircle
} from '@lucide/angular';
import { PostsService } from '../../services/posts.service';
import { MarkdownService } from '../../../../shared/services/markdown.service';
import { ToastService } from '../../../../identity/notifications/services/toast.service';
import { CreatePostCommand } from '../../contracts/create-post.command';

export interface SelectedMedia {
  file: File;
  previewUrl: string;
  formattedSize: string;
}

@Component({
  selector: 'app-create-post',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    LucideImage,
    LucideUpload,
    LucideX,
    LucideEye,
    LucideEdit3,
    LucideSend,
    LucideArrowLeft,
    LucideAlertCircle
  ],
  templateUrl: './create-post.html',
  styleUrl: './create-post.css'
})
export class CreatePost {
  private postsService = inject(PostsService);
  private markdownService = inject(MarkdownService);
  private toastService = inject(ToastService);
  private router = inject(Router);

  title = signal<string>('');
  content = signal<string>('');
  mediaFiles = signal<SelectedMedia[]>([]);
  isDragOver = signal<boolean>(false);
  isPreviewActive = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  renderedMarkdown = computed(() => {
    return this.markdownService.parse(this.content());
  });

  canSubmit = computed(() => {
    return this.title().trim().length > 0 && 
           this.content().trim().length > 0 && 
           !this.isSubmitting();
  });

  togglePreview(active: boolean): void {
    this.isPreviewActive.set(active);
  }

  triggerFileInput(fileInput: HTMLInputElement): void {
    if (this.isSubmitting()) return;
    fileInput.click();
  }

  onFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.handleFiles(Array.from(input.files));
      input.value = '';
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    if (!this.isSubmitting()) {
      this.isDragOver.set(true);
    }
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver.set(false);
  }

  onFileDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver.set(false);
    if (this.isSubmitting()) return;

    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      this.handleFiles(Array.from(event.dataTransfer.files));
    }
  }

  handleFiles(files: File[]): void {
    const validFiles: SelectedMedia[] = [];

    for (const file of files) {
      if (!file.type.startsWith('image/') && !file.type.startsWith('video/')) {
        continue;
      }
      // Check max size: 50MB
      if (file.size > 50 * 1024 * 1024) {
        this.toastService.error('File too large', `${file.name} exceeds the 50MB limit.`);
        continue;
      }

      const previewUrl = URL.createObjectURL(file);
      validFiles.push({
        file,
        previewUrl,
        formattedSize: this.formatFileSize(file.size)
      });
    }

    if (validFiles.length > 0) {
      this.mediaFiles.update(current => [...current, ...validFiles]);
    }
  }

  removeMedia(index: number): void {
    if (this.isSubmitting()) return;
    this.mediaFiles.update(current => {
      const removed = current[index];
      if (removed?.previewUrl) {
        URL.revokeObjectURL(removed.previewUrl);
      }
      return current.filter((_, i) => i !== index);
    });
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i];
  }

  onSubmit(): void {
    if (!this.canSubmit()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const command: CreatePostCommand = {
      title: this.title().trim(),
      content: this.content().trim(),
      isPenned: false,
      files: this.mediaFiles().map(m => m.file)
    };

    this.postsService.createPost(command).subscribe({
      next: res => {
        this.isSubmitting.set(false);
        if (res?.isSuccess) {
          this.toastService.success('Success', 'Post created successfully!');
          this.router.navigate(['/home']);
        } else {
          this.errorMessage.set('Failed to create post. Please check the inputs and try again.');
          this.toastService.error('Error', 'Failed to create post.');
        }
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('An unexpected error occurred while publishing the post.');
        this.toastService.error('Error', 'An unexpected error occurred.');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/home']);
  }
}

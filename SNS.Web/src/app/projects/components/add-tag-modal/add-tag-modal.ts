import { Component, Input, OnInit, inject, signal, output, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideX, LucideSearch, LucidePlus, LucideCheck } from '@lucide/angular';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { ProjectTagsService } from '../../services/project-tags.service';
import { TagDto } from '../../../shared/contracts/tag.dto';
import { ProjectTagDto } from '../../contracts/project-tag.dto';
import { CircleLoader } from '../../../shared/Loading/components/circle-loader/circle-loader';

@Component({
  selector: 'app-add-tag-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    CircleLoader,
    LucideX,
    LucideSearch,
    LucidePlus,
    LucideCheck
  ],
  templateUrl: './add-tag-modal.html',
  styleUrl: './add-tag-modal.css'
})
export class AddTagModal implements OnInit {
  @Input() projectId?: string;
  @Input() existingTagIds: string[] = [];

  readonly tagAdded = output<ProjectTagDto>();
  readonly closeModal = output<void>();

  private projectTagsService = inject(ProjectTagsService);

  searchQuery = signal<string>('');
  suggestions = signal<TagDto[]>([]);
  selectedTag = signal<TagDto | null>(null);
  isLoadingSuggestions = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  private searchSubject = new Subject<string>();

  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    if (this.isSubmitting()) return;
    this.onClose();
  }

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(query => {
        this.isLoadingSuggestions.set(true);
        return this.projectTagsService.getTags(query);
      })
    ).subscribe({
      next: res => {
        this.isLoadingSuggestions.set(false);
        if (res?.isSuccess && res.value) {
          this.suggestions.set(res.value);
        } else {
          this.suggestions.set([]);
        }
      },
      error: () => {
        this.isLoadingSuggestions.set(false);
        this.suggestions.set([]);
      }
    });

    // Initial load of common tags
    this.onSearchChange('');
  }

  onSearchChange(val: string): void {
    this.searchQuery.set(val);
    this.searchSubject.next(val);
  }

  selectTag(tag: TagDto): void {
    if (this.isAlreadyAdded(tag.id)) return;
    this.selectedTag.set(tag);
    this.errorMessage.set(null);
  }

  isAlreadyAdded(tagId: string): boolean {
    return this.existingTagIds.some(id => id?.toLowerCase() === tagId?.toLowerCase());
  }

  onAddTag(): void {
    const tag = this.selectedTag();
    if (!tag || !tag.id || this.isSubmitting()) return;

    if (!this.projectId) {
      const newTag: ProjectTagDto = {
        tagId: tag.id,
        tagName: tag.name
      };
      this.tagAdded.emit(newTag);
      this.onClose();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.projectTagsService.addProjectTag(this.projectId, {
      projectId: this.projectId,
      tagId: tag.id
    }).subscribe({
      next: res => {
        this.isSubmitting.set(false);
        if (res?.isSuccess) {
          const newTag: ProjectTagDto = {
            tagId: tag.id,
            tagName: tag.name
          };
          this.tagAdded.emit(newTag);
          this.onClose();
        } else {
          this.errorMessage.set('Failed to add tag.');
        }
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('An error occurred while adding the tag.');
      }
    });
  }

  onClose(): void {
    if (this.isSubmitting()) return;
    this.closeModal.emit();
  }

  onBackdropClick(): void {
    if (this.isSubmitting()) return;
    this.onClose();
  }
}

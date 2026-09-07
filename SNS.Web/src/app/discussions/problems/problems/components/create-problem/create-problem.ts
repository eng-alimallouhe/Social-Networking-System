import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { 
  LucideArrowLeft, 
  LucidePlus, 
  LucideTrash2, 
  LucideMoveUp, 
  LucideMoveDown, 
  LucideFileCode, 
  LucideImage, 
  LucideVideo, 
  LucideFileText, 
  LucideX, 
  LucideSend,
  LucideAlertCircle
} from '@lucide/angular';
import { ProblemsService } from '../../services/problems.service';
import { ToastService } from '../../../../../identity/notifications/services/toast.service';
import { DifficultyLevel } from '../../../../shared/enums/difficulty-level.enum';
import { ProblemBlockType } from '../../../enums/problem-block-type.enum';
import { CreateProblemCommand } from '../../contracts/create-problem.command';
import { AddTagModal } from '../../../../../projects/components/add-tag-modal/add-tag-modal';
import { ProjectTagDto } from '../../../../../projects/contracts/project-tag.dto';

export interface EditableBlock {
  id: string;
  type: ProblemBlockType;
  content: string;
  extraInfo: string;
}

@Component({
  selector: 'app-create-problem',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    AddTagModal,
    LucideArrowLeft,
    LucidePlus,
    LucideTrash2,
    LucideMoveUp,
    LucideMoveDown,
    LucideFileCode,
    LucideImage,
    LucideVideo,
    LucideFileText,
    LucideX,
    LucideSend,
    LucideAlertCircle
  ],
  templateUrl: './create-problem.html',
  styleUrl: './create-problem.css'
})
export class CreateProblem {
  private problemsService = inject(ProblemsService);
  private toastService = inject(ToastService);
  private router = inject(Router);

  DifficultyLevel = DifficultyLevel;
  ProblemBlockType = ProblemBlockType;

  title = signal<string>('');
  level = signal<DifficultyLevel>(DifficultyLevel.Easy);
  contentBlocks = signal<EditableBlock[]>([
    {
      id: this.generateBlockId(),
      type: ProblemBlockType.Text,
      content: '',
      extraInfo: ''
    }
  ]);
  selectedTags = signal<ProjectTagDto[]>([]);
  isAddTagModalOpen = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  levels = [
    { value: DifficultyLevel.Easy, label: 'Problem.Level.Easy' },
    { value: DifficultyLevel.Medium, label: 'Problem.Level.Medium' },
    { value: DifficultyLevel.Hard, label: 'Problem.Level.Hard' }
  ];

  blockTypes = [
    { type: ProblemBlockType.Text, label: 'Text (Markdown)', icon: 'file-text' },
    { type: ProblemBlockType.Code, label: 'Code', icon: 'code' },
    { type: ProblemBlockType.Image, label: 'Image URL', icon: 'image' },
    { type: ProblemBlockType.Video, label: 'Video URL', icon: 'video' }
  ];

  existingTagIds = computed(() => {
    return this.selectedTags().map(t => t.tagId);
  });

  canSubmit = computed(() => {
    const hasTitle = this.title().trim().length > 0;
    const hasValidBlock = this.contentBlocks().some(b => b.content.trim().length > 0);
    return hasTitle && hasValidBlock && !this.isSubmitting();
  });

  setLevel(newLevel: DifficultyLevel): void {
    this.level.set(newLevel);
  }

  addBlock(type: ProblemBlockType = ProblemBlockType.Text): void {
    const newBlock: EditableBlock = {
      id: this.generateBlockId(),
      type,
      content: '',
      extraInfo: type === ProblemBlockType.Code ? 'typescript' : ''
    };
    this.contentBlocks.update(blocks => [...blocks, newBlock]);
  }

  removeBlock(index: number): void {
    if (this.contentBlocks().length <= 1) {
      this.toastService.warning('Warning', 'A problem must have at least one content block.');
      return;
    }
    this.contentBlocks.update(blocks => blocks.filter((_, i) => i !== index));
  }

  moveBlockUp(index: number): void {
    if (index === 0) return;
    this.contentBlocks.update(blocks => {
      const updated = [...blocks];
      const temp = updated[index];
      updated[index] = updated[index - 1];
      updated[index - 1] = temp;
      return updated;
    });
  }

  moveBlockDown(index: number): void {
    if (index >= this.contentBlocks().length - 1) return;
    this.contentBlocks.update(blocks => {
      const updated = [...blocks];
      const temp = updated[index];
      updated[index] = updated[index + 1];
      updated[index + 1] = temp;
      return updated;
    });
  }

  changeBlockType(index: number, newType: ProblemBlockType): void {
    this.contentBlocks.update(blocks => {
      const updated = [...blocks];
      updated[index] = {
        ...updated[index],
        type: newType,
        extraInfo: newType === ProblemBlockType.Code && !updated[index].extraInfo ? 'typescript' : updated[index].extraInfo
      };
      return updated;
    });
  }

  openAddTagModal(): void {
    this.isAddTagModalOpen.set(true);
  }

  closeAddTagModal(): void {
    this.isAddTagModalOpen.set(false);
  }

  onTagAdded(tag: ProjectTagDto): void {
    if (!this.selectedTags().some(t => t.tagId.toLowerCase() === tag.tagId.toLowerCase())) {
      this.selectedTags.update(tags => [...tags, tag]);
    }
    this.closeAddTagModal();
  }

  removeTag(tagId: string): void {
    this.selectedTags.update(tags => tags.filter(t => t.tagId !== tagId));
  }

  onSubmit(): void {
    if (!this.canSubmit()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const blocksPayload = this.contentBlocks()
      .filter(b => b.content.trim().length > 0)
      .map((b, idx) => ({
        type: b.type,
        content: b.content.trim(),
        extraInfo: b.extraInfo?.trim() || null,
        order: idx + 1
      }));

    const command: CreateProblemCommand = {
      title: this.title().trim(),
      level: this.level(),
      communityId: null,
      contentBlocks: blocksPayload,
      tagIds: this.selectedTags().map(t => t.tagId)
    };

    this.problemsService.createProblem(command).subscribe({
      next: res => {
        this.isSubmitting.set(false);
        if (res?.isSuccess) {
          this.toastService.success('Success', 'Problem created successfully!');
          if (res.value) {
            this.router.navigate(['/home/problems', res.value]);
          } else {
            this.router.navigate(['/home/problems']);
          }
        } else {
          this.errorMessage.set('Failed to create problem. Please review your inputs.');
          this.toastService.error('Error', 'Failed to create problem.');
        }
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('An error occurred while creating the problem.');
        this.toastService.error('Error', 'An unexpected error occurred.');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/home/problems']);
  }

  private generateBlockId(): string {
    return 'block-' + Math.random().toString(36).substring(2, 9);
  }
}

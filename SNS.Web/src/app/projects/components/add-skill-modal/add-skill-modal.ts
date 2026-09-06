import { Component, Input, OnInit, inject, signal, output, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideX, LucideSearch, LucidePlus, LucideCheck } from '@lucide/angular';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { ProjectService } from '../../services/project.service';
import { ProjectSkillsService } from '../../services/project-skills.service';
import { ProjectSkillDto } from '../../contracts/project-skill.dto';
import { CircleLoader } from '../../../shared/Loading/components/circle-loader/circle-loader';

@Component({
  selector: 'app-add-skill-modal',
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
  templateUrl: './add-skill-modal.html',
  styleUrl: './add-skill-modal.css'
})
export class AddSkillModal implements OnInit {
  @Input({ required: true }) projectId!: string;
  @Input() existingSkillIds: string[] = [];

  readonly skillAdded = output<ProjectSkillDto>();
  readonly closeModal = output<void>();

  private projectService = inject(ProjectService);
  private projectSkillsService = inject(ProjectSkillsService);

  searchQuery = signal<string>('');
  suggestions = signal<ProjectSkillDto[]>([]);
  selectedSkill = signal<ProjectSkillDto | null>(null);
  isLoadingSuggestions = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  private searchSubject = new Subject<string>();

  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    this.onClose();
  }

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(query => {
        this.isLoadingSuggestions.set(true);
        return this.projectService.searchSkills(query);
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

    // Initial load of common skills
    this.onSearchChange('');
  }

  onSearchChange(val: string): void {
    this.searchQuery.set(val);
    this.searchSubject.next(val);
  }

  selectSkill(skill: ProjectSkillDto): void {
    if (this.isAlreadyAdded(skill.skillId)) return;
    this.selectedSkill.set(skill);
    this.errorMessage.set(null);
  }

  isAlreadyAdded(skillId: string): boolean {
    return this.existingSkillIds.some(id => id?.toLowerCase() === skillId?.toLowerCase());
  }

  onAddSkill(): void {
    const skill = this.selectedSkill();
    if (!skill || !this.projectId || this.isSubmitting()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.projectSkillsService.addProjectSkill(this.projectId, {
      projectId: this.projectId,
      skillId: skill.skillId
    }).subscribe({
      next: res => {
        this.isSubmitting.set(false);
        if (res?.isSuccess) {
          this.skillAdded.emit(skill);
          this.onClose();
        } else {
          this.errorMessage.set('Failed to add skill.');
        }
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('An error occurred while adding the skill.');
      }
    });
  }

  onClose(): void {
    this.closeModal.emit();
  }
}

import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { 
  LucideArrowLeft, 
  LucidePlus, 
  LucideX, 
  LucideSend, 
  LucideAlertCircle, 
  LucideGitBranch,
  LucideGlobe
} from '@lucide/angular';
import { ProjectService } from '../../services/project.service';
import { ToastService } from '../../../identity/notifications/services/toast.service';
import { ProjectType } from '../../enums/project-type.enum';
import { CreateProjectCommand } from '../../contracts/create-project.command';
import { AddSkillModal } from '../add-skill-modal/add-skill-modal';
import { AddTagModal } from '../add-tag-modal/add-tag-modal';
import { ProjectSkillDto } from '../../contracts/project-skill.dto';
import { ProjectTagDto } from '../../contracts/project-tag.dto';

@Component({
  selector: 'app-create-project',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    AddSkillModal,
    AddTagModal,
    LucideArrowLeft,
    LucidePlus,
    LucideX,
    LucideSend,
    LucideAlertCircle,
    LucideGitBranch,
    LucideGlobe
  ],
  templateUrl: './create-project.html',
  styleUrl: './create-project.css'
})
export class CreateProject {
  private projectService = inject(ProjectService);
  private toastService = inject(ToastService);
  private router = inject(Router);

  ProjectType = ProjectType;

  title = signal<string>('');
  shortDescription = signal<string>('');
  type = signal<ProjectType>(ProjectType.OpenSource);
  gitHubUrl = signal<string>('');
  liveDemoUrl = signal<string>('');
  selectedSkills = signal<ProjectSkillDto[]>([]);
  selectedTags = signal<ProjectTagDto[]>([]);

  isAddSkillModalOpen = signal<boolean>(false);
  isAddTagModalOpen = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  projectTypes = [
    { value: ProjectType.OpenSource, label: 'Project.Type.OpenSource' },
    { value: ProjectType.Personal, label: 'Project.Type.Personal' },
    { value: ProjectType.Commercial, label: 'Project.Type.Commercial' },
    { value: ProjectType.Academic, label: 'Project.Type.Academic' }
  ];

  existingSkillIds = computed(() => {
    return this.selectedSkills().map(s => s.skillId);
  });

  existingTagIds = computed(() => {
    return this.selectedTags().map(t => t.tagId);
  });

  canSubmit = computed(() => {
    return this.title().trim().length > 0 && 
           this.shortDescription().trim().length > 0 && 
           !this.isSubmitting();
  });

  setProjectType(newType: ProjectType): void {
    this.type.set(newType);
  }

  openAddSkillModal(): void {
    this.isAddSkillModalOpen.set(true);
  }

  closeAddSkillModal(): void {
    this.isAddSkillModalOpen.set(false);
  }

  onSkillAdded(skill: ProjectSkillDto): void {
    if (!this.selectedSkills().some(s => s.skillId.toLowerCase() === skill.skillId.toLowerCase())) {
      this.selectedSkills.update(skills => [...skills, skill]);
    }
    this.closeAddSkillModal();
  }

  removeSkill(skillId: string): void {
    this.selectedSkills.update(skills => skills.filter(s => s.skillId !== skillId));
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

    const command: CreateProjectCommand = {
      title: this.title().trim(),
      shortDescription: this.shortDescription().trim(),
      gitHubUrl: this.gitHubUrl().trim(),
      liveDemoUrl: this.liveDemoUrl().trim(),
      type: this.type(),
      skillIds: this.selectedSkills().map(s => s.skillId),
      tagIds: this.selectedTags().map(t => t.tagId)
    };

    this.projectService.createProject(command).subscribe({
      next: res => {
        this.isSubmitting.set(false);
        if (res?.isSuccess && res.value) {
          this.toastService.success('Success', 'Project created successfully!');
          // Navigate immediately to the newly created project details
          this.router.navigate(['/projects', res.value]);
        } else {
          this.errorMessage.set('Failed to create project. Please verify inputs.');
          this.toastService.error('Error', 'Failed to create project.');
        }
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('An unexpected error occurred while creating the project.');
        this.toastService.error('Error', 'An unexpected error occurred.');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/projects']);
  }
}

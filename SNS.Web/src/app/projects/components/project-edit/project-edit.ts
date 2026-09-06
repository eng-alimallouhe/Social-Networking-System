import { Component, OnInit, OnDestroy, inject, signal, computed, ElementRef, HostListener } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import {
  LucideArrowLeft,
  LucideInfo,
  LucideCode,
  LucideTag,
  LucideUsers,
  LucideImage,
  LucideGitBranch,
  LucideSettings,
  LucideSave,
  LucidePlus,
  LucideTrash2,
  LucideX,
  LucideCheck,
  LucideSearch,
  LucideChevronDown,
  LucideExternalLink,
  LucideClock,
  LucideUpload,
  LucideFolder,
  LucideFileText,
  LucideBookOpen
} from '@lucide/angular';
import { TagDto } from '../../../shared/contracts/tag.dto';
import { LanguageService } from '../../../shared/services/language.service';
import { SupportedLanguage } from '../../../shared/contracts/supported-language.enum';
import { ProjectService } from '../../services/project.service';
import { ProjectSkillsService } from '../../services/project-skills.service';
import { ProjectTagsService } from '../../services/project-tags.service';
import { ProjectContributorsService } from '../../services/project-contributors.service';
import { ProjectMediaService } from '../../services/project-media.service';
import { AuthenticationService } from '../../../identity/shared/services/authentication.service';
import { ToastService } from '../../../identity/notifications/services/toast.service';
import { ProjectDetailsDto } from '../../contracts/project-details.dto';
import { ProjectSkillDto } from '../../contracts/project-skill.dto';
import { ProjectTagDto } from '../../contracts/project-tag.dto';
import { ProjectContributorManagementDto } from '../../contracts/project-contributor-management.dto';
import { ProjectMediaDto } from '../../contracts/project-media.dto';
import { FileNode } from '../../contracts/file-node.dto';
import { ProjectStatus } from '../../enums/project-status.enum';
import { ProjectType } from '../../enums/project-type.enum';
import { InvitingStatus } from '../../enums/inviting-status.enum';
import { CircleLoader } from '../../../shared/Loading/components/circle-loader/circle-loader';
import { AddSkillModal } from '../add-skill-modal/add-skill-modal';
import { AddContributorModal } from '../add-contributor-modal/add-contributor-modal';
import { AppTextarea } from '../../../shared/design-system/components/app-textarea/app-textarea';

export type ProjectEditTab = 'general' | 'readme' | 'skills' | 'tags' | 'contributors' | 'media' | 'source-code' | 'settings';

@Component({
  selector: 'app-project-edit',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    CircleLoader,
    AddSkillModal,
    AddContributorModal,
    AppTextarea,
    LucideArrowLeft,
    LucideInfo,
    LucideCode,
    LucideTag,
    LucideUsers,
    LucideImage,
    LucideGitBranch,
    LucideSettings,
    LucideSave,
    LucidePlus,
    LucideTrash2,
    LucideX,
    LucideCheck,
    LucideSearch,
    LucideChevronDown,
    LucideExternalLink,
    LucideClock,
    LucideUpload,
    LucideFolder,
    LucideFileText,
    LucideBookOpen
  ],
  templateUrl: './project-edit.html',
  styleUrl: './project-edit.css'
})
export class ProjectEdit implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private location = inject(Location);
  private projectService = inject(ProjectService);
  private projectSkillsService = inject(ProjectSkillsService);
  private projectTagsService = inject(ProjectTagsService);
  private projectContributorsService = inject(ProjectContributorsService);
  private projectMediaService = inject(ProjectMediaService);
  private authService = inject(AuthenticationService);
  private toastService = inject(ToastService);
  private translate = inject(TranslateService);
  private languageService = inject(LanguageService);

  private elementRef = inject(ElementRef);

  readonly defaultAvatar = 'assets/images/default-avatar.png';
  readonly InvitingStatus = InvitingStatus;
  readonly ProjectStatus = ProjectStatus;

  // Allowed status transitions (Draft disallowed for existing projects per domain rule)
  readonly allowedStatuses: ProjectStatus[] = [
    ProjectStatus.Ongoing,
    ProjectStatus.Completed,
    ProjectStatus.Published,
    ProjectStatus.Archived
  ];

  projectId = signal<string>('');
  project = signal<ProjectDetailsDto | null>(null);
  activeTab = signal<ProjectEditTab>('general');
  isSectionDropdownOpen = signal<boolean>(false);

  isRtl = computed(() => {
    return this.languageService.currentLanguage() === SupportedLanguage.Arabic ||
      (typeof document !== 'undefined' && document.documentElement.dir === 'rtl');
  });

  isLoadingProject = signal<boolean>(true);
  isSavingGeneral = signal<boolean>(false);
  isSavingReadme = signal<boolean>(false);
  isSavingStatus = signal<boolean>(false);

  // General tab form models
  generalTitle = signal<string>('');
  generalShortDesc = signal<string>('');
  generalLiveDemo = signal<string>('');
  generalGithubUrl = signal<string>('');
  generalReadme = signal<string>('');

  // Skills tab
  skills = signal<ProjectSkillDto[]>([]);
  isLoadingSkills = signal<boolean>(false);
  isAddSkillModalOpen = signal<boolean>(false);

  // Tags tab
  tags = signal<ProjectTagDto[]>([]);
  isAddTagOpen = signal<boolean>(false);
  tagSearchQuery = signal<string>('');
  tagSuggestions = signal<TagDto[]>([]);
  selectedTag = signal<TagDto | null>(null);
  isLoadingTagSuggestions = signal<boolean>(false);
  isAddingTag = signal<boolean>(false);
  private tagSearchSubject = new Subject<string>();
  private tagSearchSubscription?: Subscription;

  // Contributors tab
  contributors = signal<ProjectContributorManagementDto[]>([]);
  isLoadingContributors = signal<boolean>(false);
  isAddContributorModalOpen = signal<boolean>(false);

  // Media tab
  mediaList = signal<ProjectMediaDto[]>([]);
  isLoadingMedia = signal<boolean>(false);
  newMediaCaption = signal<string>('');
  selectedMediaFile = signal<File | null>(null);
  isUploadingMedia = signal<boolean>(false);

  // Source code tab
  sourceCodeTree = signal<FileNode[]>([]);
  isLoadingSourceCode = signal<boolean>(false);

  // Settings tab
  selectedStatus = signal<ProjectStatus>(ProjectStatus.Ongoing);

  readonly isOwner = computed(() => {
    if (!this.authService.isAuthenticated()) return false;
    const currentId = this.authService.getProfileId();
    if (!currentId) return false;
    const proj = this.project();
    return !!(proj?.ownerId && String(proj.ownerId).toLowerCase() === currentId.toLowerCase());
  });

  readonly existingSkillIds = computed(() => this.skills().map(s => s.skillId));

  readonly acceptedContributors = computed(() =>
    this.contributors().filter(c => c.invitingStatus === InvitingStatus.Accepted)
  );

  readonly pendingContributors = computed(() =>
    this.contributors().filter(c => c.invitingStatus === InvitingStatus.Pending)
  );

  ngOnInit(): void {
    this.tagSearchSubscription = this.tagSearchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(query => {
        this.isLoadingTagSuggestions.set(true);
        return this.projectTagsService.getTags(query);
      })
    ).subscribe({
      next: res => {
        this.isLoadingTagSuggestions.set(false);
        if (res?.isSuccess && res.value) {
          this.tagSuggestions.set(res.value);
        } else {
          this.tagSuggestions.set([]);
        }
      },
      error: () => {
        this.isLoadingTagSuggestions.set(false);
        this.tagSuggestions.set([]);
      }
    });

    this.route.paramMap.subscribe(params => {
      const id = params.get('projectId') || params.get('id');
      if (id) {
        this.projectId.set(id);
        this.loadProjectDetails();
      }
    });
  }

  loadProjectDetails(): void {
    const id = this.projectId();
    if (!id) return;

    this.isLoadingProject.set(true);
    this.projectService.getProjectById(id).subscribe({
      next: res => {
        this.isLoadingProject.set(false);
        if (res?.isSuccess && res.value) {
          const p = res.value;
          this.project.set(p);

          // Verify ownership UX guard
          if (!this.isOwner()) {
            this.router.navigate(['/projects', id]);
            return;
          }

          // Populate form fields
          this.generalTitle.set(p.title || '');
          this.generalShortDesc.set(p.shortDescription || '');
          this.generalLiveDemo.set(p.liveDemoUrl || '');
          this.generalGithubUrl.set(p.gitHubUrl || '');
          this.generalReadme.set(p.readmeContent || '');
          this.skills.set(p.skills || []);
          this.tags.set(p.tags || []);
          this.selectedStatus.set(p.status || ProjectStatus.Ongoing);
        } else {
          this.router.navigate(['/projects', id]);
        }
      },
      error: () => {
        this.isLoadingProject.set(false);
        this.router.navigate(['/projects', id]);
      }
    });
  }

  setTab(tab: ProjectEditTab): void {
    this.activeTab.set(tab);

    if (tab === 'contributors' && this.contributors().length === 0) {
      this.loadOwnerContributors();
    } else if (tab === 'media' && this.mediaList().length === 0) {
      this.loadMedia();
    } else if (tab === 'source-code' && this.sourceCodeTree().length === 0) {
      this.loadSourceCode();
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.isSectionDropdownOpen() && !this.elementRef.nativeElement.querySelector('.section-dropdown-wrap')?.contains(event.target)) {
      this.isSectionDropdownOpen.set(false);
    }
  }

  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    this.isSectionDropdownOpen.set(false);
  }

  toggleSectionDropdown(): void {
    this.isSectionDropdownOpen.set(!this.isSectionDropdownOpen());
  }

  selectSection(tab: ProjectEditTab): void {
    this.setTab(tab);
    this.isSectionDropdownOpen.set(false);
  }

  // General Tab
  saveGeneralInfo(): void {
    const id = this.projectId();
    if (!id || this.isSavingGeneral()) return;

    this.isSavingGeneral.set(true);

    this.projectService.updateProject(id, {
      projectId: id,
      title: this.generalTitle().trim(),
      shortDescription: this.generalShortDesc().trim(),
      liveDemoUrl: this.generalLiveDemo().trim()
    }).subscribe({
      next: res => {
        this.isSavingGeneral.set(false);
        if (res?.isSuccess) {
          const title = this.translate.instant('ProjectEdit.Common.Saved_Title') || 'Saved';
          const msg = this.translate.instant('ProjectEdit.General.Saved_Message') || 'Project details updated successfully.';
          this.toastService.success(title, msg);
        } else {
          this.toastService.error('Error', 'Failed to update project.');
        }
      },
      error: () => {
        this.isSavingGeneral.set(false);
        this.toastService.error('Error', 'An error occurred while saving.');
      }
    });
  }

  // README Tab
  saveReadme(): void {
    const id = this.projectId();
    if (!id || this.isSavingReadme()) return;

    this.isSavingReadme.set(true);

    this.projectService.updateProjectReadme(id, {
      projectId: id,
      readmeContent: this.generalReadme()
    }).subscribe({
      next: res => {
        this.isSavingReadme.set(false);
        if (res?.isSuccess) {
          const title = this.translate.instant('ProjectEdit.Common.Saved_Title') || 'Saved';
          const msg = this.translate.instant('ProjectEdit.Readme.Saved_Message') || 'Project README updated successfully.';
          this.toastService.success(title, msg);
        } else {
          this.toastService.error('Error', 'Failed to update README.');
        }
      },
      error: () => {
        this.isSavingReadme.set(false);
        this.toastService.error('Error', 'An error occurred while saving README.');
      }
    });
  }

  // Skills Tab
  openAddSkill(): void {
    this.isAddSkillModalOpen.set(true);
  }

  closeAddSkill(): void {
    this.isAddSkillModalOpen.set(false);
  }

  onSkillAdded(newSkill: ProjectSkillDto): void {
    const alreadyExists = this.skills().some(s => s.skillId?.toLowerCase() === newSkill.skillId?.toLowerCase());
    if (!alreadyExists) {
      this.skills.set([...this.skills(), newSkill]);
    }
    const title = this.translate.instant('ProjectEdit.Common.Saved_Title') || 'Success';
    const msg = this.translate.instant('ProjectEdit.Skills.Skill_Added_Toast') || 'Skill added to project.';
    this.toastService.success(title, msg);
  }

  removeSkill(skill: ProjectSkillDto): void {
    const id = this.projectId();
    if (!id) return;

    this.isLoadingSkills.set(true);
    this.projectSkillsService.removeProjectSkill(id, skill.skillId).subscribe({
      next: res => {
        this.isLoadingSkills.set(false);
        if (res?.isSuccess) {
          this.skills.set(this.skills().filter(s => s.skillId !== skill.skillId));
          const title = this.translate.instant('ProjectEdit.Common.Saved_Title') || 'Success';
          const msg = this.translate.instant('ProjectEdit.Skills.Skill_Removed_Toast') || 'Skill removed.';
          this.toastService.success(title, msg);
        } else {
          this.toastService.error('Error', 'Failed to remove skill.');
        }
      },
      error: () => {
        this.isLoadingSkills.set(false);
        this.toastService.error('Error', 'An error occurred while removing skill.');
      }
    });
  }

  ngOnDestroy(): void {
    this.tagSearchSubscription?.unsubscribe();
    this.tagSearchSubject.complete();
  }

  // Tags Tab
  openAddTagUi(): void {
    this.isAddTagOpen.set(true);
    this.selectedTag.set(null);
    this.tagSearchQuery.set('');
    this.tagSearchSubject.next('');
  }

  closeAddTagUi(): void {
    this.isAddTagOpen.set(false);
    this.selectedTag.set(null);
    this.tagSearchQuery.set('');
    this.tagSuggestions.set([]);
  }

  toggleAddTagUi(): void {
    if (this.isAddTagOpen()) {
      this.closeAddTagUi();
    } else {
      this.openAddTagUi();
    }
  }

  onTagSearchChange(val: string): void {
    this.tagSearchQuery.set(val);
    this.tagSearchSubject.next(val);
  }

  onTagInputEnter(event: Event): void {
    if (this.selectedTag()) {
      event.preventDefault();
      this.addTag();
    }
  }

  selectTag(tag: TagDto): void {
    if (this.isTagAlreadyAdded(tag.id)) return;
    this.selectedTag.set(tag);
  }

  clearSelectedTag(): void {
    this.selectedTag.set(null);
  }

  isTagAlreadyAdded(tagId: string): boolean {
    return this.tags().some(t => t.tagId?.toLowerCase() === tagId?.toLowerCase());
  }

  addTag(): void {
    const id = this.projectId();
    const selected = this.selectedTag();
    if (!id || !selected || !selected.id || this.isAddingTag()) return;

    this.isAddingTag.set(true);
    this.projectTagsService.addProjectTag(id, { projectId: id, tagId: selected.id }).subscribe({
      next: res => {
        this.isAddingTag.set(false);
        if (res?.isSuccess) {
          const newTag: ProjectTagDto = {
            tagId: selected.id,
            tagName: selected.name
          };
          this.tags.set([...this.tags(), newTag]);
          this.selectedTag.set(null);
          this.tagSearchQuery.set('');
          this.tagSuggestions.set([]);
          this.isAddTagOpen.set(false);
          const title = this.translate.instant('ProjectEdit.Common.Saved_Title') || 'Success';
          const msg = this.translate.instant('ProjectEdit.Tags.Tag_Added_Toast') || 'Tag added.';
          this.toastService.success(title, msg);
        } else {
          this.toastService.error('Error', 'Failed to add tag.');
        }
      },
      error: () => {
        this.isAddingTag.set(false);
        this.toastService.error('Error', 'An error occurred while adding tag.');
      }
    });
  }

  removeTag(tag: ProjectTagDto): void {
    const id = this.projectId();
    if (!id) return;

    this.projectTagsService.removeProjectTag(id, tag.tagId).subscribe({
      next: res => {
        if (res?.isSuccess) {
          this.tags.set(this.tags().filter(t => t.tagId !== tag.tagId));
          const title = this.translate.instant('ProjectEdit.Common.Saved_Title') || 'Success';
          const msg = this.translate.instant('ProjectEdit.Tags.Tag_Removed_Toast') || 'Tag removed.';
          this.toastService.success(title, msg);
        } else {
          this.toastService.error('Error', 'Failed to remove tag.');
        }
      },
      error: () => this.toastService.error('Error', 'An error occurred while removing tag.')
    });
  }

  // Contributors Tab
  loadOwnerContributors(): void {
    const id = this.projectId();
    if (!id) return;

    this.isLoadingContributors.set(true);
    this.projectContributorsService.getProjectParticipantsForOwner(id, 1, 50).subscribe({
      next: res => {
        this.isLoadingContributors.set(false);
        if (res?.isSuccess && res.value?.items) {
          this.contributors.set(res.value.items);
        }
      },
      error: () => this.isLoadingContributors.set(false)
    });
  }

  openAddContributor(): void {
    this.isAddContributorModalOpen.set(true);
  }

  closeAddContributor(): void {
    this.isAddContributorModalOpen.set(false);
  }

  onContributorInvited(): void {
    this.loadOwnerContributors();
  }

  removeContributorOrInvitation(contributor: ProjectContributorManagementDto): void {
    const id = this.projectId();
    if (!id) return;

    this.projectContributorsService.removeProjectContributor(id, contributor.profileId).subscribe({
      next: res => {
        if (res?.isSuccess) {
          this.contributors.set(this.contributors().filter(c => c.profileId !== contributor.profileId));
          const isPending = contributor.invitingStatus === InvitingStatus.Pending;
          const msg = isPending
            ? (this.translate.instant('ProjectEdit.Contributors.Invitation_Canceled_Toast') || 'Invitation canceled.')
            : (this.translate.instant('ProjectEdit.Contributors.Contributor_Removed_Toast') || 'Contributor removed.');
          this.toastService.success(this.translate.instant('ProjectEdit.Common.Saved_Title') || 'Success', msg);
        } else {
          this.toastService.error('Error', 'Failed to update contributor.');
        }
      },
      error: () => this.toastService.error('Error', 'An error occurred.')
    });
  }

  // Media Tab
  loadMedia(): void {
    const id = this.projectId();
    if (!id) return;

    this.isLoadingMedia.set(true);
    this.projectService.getProjectMedia(id, 1, 50).subscribe({
      next: res => {
        this.isLoadingMedia.set(false);
        if (res?.isSuccess && res.value?.items) {
          this.mediaList.set(res.value.items);
        }
      },
      error: () => this.isLoadingMedia.set(false)
    });
  }

  onMediaFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input?.files && input.files[0]) {
      this.selectedMediaFile.set(input.files[0]);
    }
  }

  uploadMedia(): void {
    const id = this.projectId();
    const file = this.selectedMediaFile();
    if (!id || !file || this.isUploadingMedia()) return;

    this.isUploadingMedia.set(true);
    this.projectMediaService.addProjectMedia(id, file, this.newMediaCaption().trim(), 'Image').subscribe({
      next: res => {
        this.isUploadingMedia.set(false);
        if (res?.isSuccess) {
          this.selectedMediaFile.set(null);
          this.newMediaCaption.set('');
          this.loadMedia();
          this.toastService.success(
            this.translate.instant('ProjectEdit.Common.Saved_Title') || 'Success',
            this.translate.instant('ProjectEdit.Media.Uploaded_Toast') || 'Media uploaded.'
          );
        } else {
          this.toastService.error('Error', 'Failed to upload media.');
        }
      },
      error: () => {
        this.isUploadingMedia.set(false);
        this.toastService.error('Error', 'An error occurred while uploading media.');
      }
    });
  }

  deleteMedia(media: ProjectMediaDto): void {
    const id = this.projectId();
    if (!id) return;

    this.projectMediaService.deleteProjectMedia(id, media.mediaId).subscribe({
      next: res => {
        if (res?.isSuccess) {
          this.mediaList.set(this.mediaList().filter(m => m.mediaId !== media.mediaId));
          this.toastService.success(
            this.translate.instant('ProjectEdit.Common.Saved_Title') || 'Success',
            this.translate.instant('ProjectEdit.Media.Deleted_Toast') || 'Media deleted.'
          );
        } else {
          this.toastService.error('Error', 'Failed to delete media.');
        }
      },
      error: () => this.toastService.error('Error', 'An error occurred while deleting media.')
    });
  }

  // Source Code Tab
  loadSourceCode(): void {
    const id = this.projectId();
    if (!id) return;

    this.isLoadingSourceCode.set(true);
    this.projectService.getProjectSourceCode(id).subscribe({
      next: res => {
        this.isLoadingSourceCode.set(false);
        if (res?.isSuccess && res.value) {
          this.sourceCodeTree.set(res.value);
        }
      },
      error: () => this.isLoadingSourceCode.set(false)
    });
  }

  // Status Tab
  saveProjectStatus(): void {
    const id = this.projectId();
    const newStatus = this.selectedStatus();
    if (!id || this.isSavingStatus()) return;

    this.isSavingStatus.set(true);
    this.projectService.changeProjectStatus(id, {
      projectId: id,
      status: newStatus
    }).subscribe({
      next: res => {
        this.isSavingStatus.set(false);
        if (res?.isSuccess) {
          const current = this.project();
          if (current) {
            this.project.set({ ...current, status: newStatus });
          }
          this.toastService.success(
            this.translate.instant('ProjectEdit.Common.Saved_Title') || 'Success',
            this.translate.instant('ProjectEdit.Settings.Status_Changed_Toast') || 'Project status updated.'
          );
        } else {
          this.toastService.error('Error', 'Invalid status transition.');
        }
      },
      error: () => {
        this.isSavingStatus.set(false);
        this.toastService.error('Error', 'An error occurred while changing project status.');
      }
    });
  }

  goBack(): void {
    const id = this.projectId();
    if (id) {
      this.router.navigate(['/projects', id]);
    } else {
      this.location.back();
    }
  }

  onAvatarError(event: Event): void {
    const img = event.target as HTMLImageElement;
    if (img) img.src = this.defaultAvatar;
  }
}

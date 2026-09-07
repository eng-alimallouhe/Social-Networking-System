import { Component, OnInit, inject, signal, computed, ElementRef, HostListener, ViewChild } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import {
  LucideArrowLeft,
  LucideInfo,
  LucideGlobe,
  LucideCode,
  LucideSave,
  LucidePlus,
  LucideX,
  LucideCheck,
  LucideChevronDown,
  LucideUpload,
  LucideCamera
} from '@lucide/angular';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportedLanguage } from '../../../../shared/contracts/supported-language.enum';
import { ProfilesService } from '../../services/profiles.service';
import { AuthenticationService } from '../../../../identity/shared/services/authentication.service';
import { ToastService } from '../../../../identity/notifications/services/toast.service';
import { ProfileDetailsDto } from '../../contracts/profile-details.dto';
import { ProfileSkillDto } from '../../contracts/profile-skill.dto';
import { ProjectSkillDto } from '../../../../projects/contracts/project-skill.dto';
import { CircleLoader } from '../../../../shared/Loading/components/circle-loader/circle-loader';
import { AddSkillModal } from '../../../../projects/components/add-skill-modal/add-skill-modal';
import { AppInput } from '../../../../shared/design-system/components/app-input/app-input';
import { AppTextarea } from '../../../../shared/design-system/components/app-textarea/app-textarea';
import { AppAvatar } from '../../../../shared/design-system/components/app-avatar/app-avatar';
import { AppSelect, SelectOption } from '../../../../shared/design-system/components/app-select/app-select';
import { ALL_SPECIALIZATIONS } from '../create-profile/create-profile';

export type ProfileEditTab = 'general' | 'picture' | 'social' | 'skills';

@Component({
  selector: 'app-profile-edit',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    CircleLoader,
    AddSkillModal,
    AppInput,
    AppTextarea,
    AppAvatar,
    AppSelect,
    LucideArrowLeft,
    LucideInfo,
    LucideGlobe,
    LucideCode,
    LucideSave,
    LucidePlus,
    LucideX,
    LucideCheck,
    LucideChevronDown,
    LucideUpload,
    LucideCamera
  ],
  templateUrl: './profile-edit.html',
  styleUrl: './profile-edit.css'
})
export class ProfileEdit implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private location = inject(Location);
  private profilesService = inject(ProfilesService);
  private authService = inject(AuthenticationService);
  private toastService = inject(ToastService);
  private translate = inject(TranslateService);
  private languageService = inject(LanguageService);
  private elementRef = inject(ElementRef);

  profileId = signal<string>('');
  profile = signal<ProfileDetailsDto | null>(null);
  activeTab = signal<ProfileEditTab>('general');
  isSectionDropdownOpen = signal<boolean>(false);

  isLoadingProfile = signal<boolean>(true);

  // General tab
  fullName = signal<string>('');
  specialization = signal<string>('');
  specializationOptions = computed<SelectOption[]>(() =>
    ALL_SPECIALIZATIONS.map(s => ({ value: s, label: s }))
  );
  locationValue = signal<string>('');
  bio = signal<string>('');
  isSavingGeneral = signal<boolean>(false);

  // Picture tab
  @ViewChild('fileInput') fileInputRef?: ElementRef<HTMLInputElement>;
  selectedPictureFile = signal<File | null>(null);
  previewPictureUrl = signal<string | null>(null);
  isUploadingPicture = signal<boolean>(false);
  isDragOver = signal<boolean>(false);

  // Social tab
  facebookUrl = signal<string>('');
  linkedInUrl = signal<string>('');
  gitHubUrl = signal<string>('');
  xUrl = signal<string>('');
  website = signal<string>('');
  isSavingSocial = signal<boolean>(false);

  // Skills tab
  skills = signal<ProfileSkillDto[]>([]);
  isAddSkillModalOpen = signal<boolean>(false);
  removingSkillId = signal<string | null>(null);

  isRtl = computed(() => {
    return this.languageService.currentLanguage() === SupportedLanguage.Arabic ||
      (typeof document !== 'undefined' && document.documentElement.dir === 'rtl');
  });

  readonly isOwner = computed(() => {
    if (!this.authService.isAuthenticated()) return false;
    const currentId = this.authService.getProfileId() || this.authService.getUserId();
    if (!currentId) return false;
    const pId = this.profileId();
    return !!(pId && currentId.toLowerCase() === pId.toLowerCase());
  });

  readonly existingSkillIds = computed(() => this.skills().map(s => s.skillId));

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('profileId');
      if (id) {
        this.profileId.set(id);
        this.loadProfile(id);
      }
    });
  }

  loadProfile(id: string): void {
    this.isLoadingProfile.set(true);
    this.profilesService.getProfileById(id).subscribe({
      next: res => {
        this.isLoadingProfile.set(false);
        if (res?.isSuccess && res.value) {
          const p = res.value;
          this.profile.set(p);

          // Populate general form
          this.fullName.set(p.fullName || '');
          this.specialization.set(p.specialization || '');
          this.locationValue.set(p.location || '');
          this.bio.set(p.bio || '');

          // Populate picture
          this.previewPictureUrl.set(p.profilePictureUrl || null);

          // Populate social form
          this.facebookUrl.set(p.facebookUrl || '');
          this.linkedInUrl.set(p.linkedInUrl || '');
          this.gitHubUrl.set(p.gitHubUrl || '');
          this.xUrl.set(p.xUrl || '');
          this.website.set(p.website || '');

          // Populate skills
          this.skills.set(p.skills || []);
        } else {
          this.toastService.error(
            this.translate.instant('Common.Error') || 'Error',
            this.translate.instant('ProfileEdit.Errors.NotFound') || 'Failed to load profile.'
          );
        }
      },
      error: () => {
        this.isLoadingProfile.set(false);
        this.toastService.error(
          this.translate.instant('Common.Error') || 'Error',
          this.translate.instant('ProfileEdit.Errors.LoadFailed') || 'Failed to load profile details.'
        );
      }
    });
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

  selectSection(tab: ProfileEditTab): void {
    this.activeTab.set(tab);
    this.isSectionDropdownOpen.set(false);
  }

  toggleSectionDropdown(): void {
    this.isSectionDropdownOpen.update(v => !v);
  }

  goBack(): void {
    this.location.back();
  }

  // 1. Save General Info
  saveGeneralInformation(): void {
    if (this.isSavingGeneral()) return;
    const name = this.fullName().trim();
    if (!name) {
      this.toastService.error(
        this.translate.instant('Common.Error') || 'Error',
        this.translate.instant('ProfileEdit.Validation.NameRequired') || 'Full Name is required.'
      );
      return;
    }

    this.isSavingGeneral.set(true);
    this.profilesService.updateBasicInformation({
      fullName: name,
      specialization: this.specialization().trim(),
      location: this.locationValue().trim(),
      bio: this.bio().trim()
    }).subscribe({
      next: res => {
        this.isSavingGeneral.set(false);
        if (res?.isSuccess) {
          this.toastService.success(
            this.translate.instant('Common.Success') || 'Success',
            this.translate.instant('ProfileEdit.Success.GeneralUpdated') || 'General information updated successfully.'
          );
          if (this.profile()) {
            this.profile.update(p => p ? {
              ...p,
              fullName: name,
              specialization: this.specialization().trim(),
              location: this.locationValue().trim(),
              bio: this.bio().trim()
            } : null);
          }
        } else {
          this.toastService.error(
            this.translate.instant('Common.Error') || 'Error',
            this.translate.instant('ProfileEdit.Errors.UpdateFailed') || 'Failed to update general information.'
          );
        }
      },
      error: () => {
        this.isSavingGeneral.set(false);
        this.toastService.error(
          this.translate.instant('Common.Error') || 'Error',
          this.translate.instant('ProfileEdit.Errors.UpdateFailed') || 'An error occurred while saving.'
        );
      }
    });
  }

  // 2. Picture Management
  triggerFileInput(): void {
    this.fileInputRef?.nativeElement?.click();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.handlePictureFile(input.files[0]);
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver.set(true);
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver.set(false);
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver.set(false);
    if (event.dataTransfer?.files && event.dataTransfer.files[0]) {
      this.handlePictureFile(event.dataTransfer.files[0]);
    }
  }

  private handlePictureFile(file: File): void {
    if (!file.type.startsWith('image/')) {
      this.toastService.error(
        this.translate.instant('Common.Error') || 'Error',
        this.translate.instant('ProfileEdit.Validation.ImageOnly') || 'Please select a valid image file.'
      );
      return;
    }
    this.selectedPictureFile.set(file);
    const reader = new FileReader();
    reader.onload = () => {
      this.previewPictureUrl.set(reader.result as string);
    };
    reader.readAsDataURL(file);
  }

  uploadProfilePicture(): void {
    const file = this.selectedPictureFile();
    if (!file || this.isUploadingPicture()) return;

    this.isUploadingPicture.set(true);
    this.profilesService.updateProfilePicture(file).subscribe({
      next: res => {
        this.isUploadingPicture.set(false);
        if (res?.isSuccess) {
          this.selectedPictureFile.set(null);
          this.toastService.success(
            this.translate.instant('Common.Success') || 'Success',
            this.translate.instant('ProfileEdit.Success.PictureUpdated') || 'Profile picture updated successfully.'
          );
        } else {
          this.toastService.error(
            this.translate.instant('Common.Error') || 'Error',
            this.translate.instant('ProfileEdit.Errors.UpdateFailed') || 'Failed to update profile picture.'
          );
        }
      },
      error: () => {
        this.isUploadingPicture.set(false);
        this.toastService.error(
          this.translate.instant('Common.Error') || 'Error',
          this.translate.instant('ProfileEdit.Errors.UpdateFailed') || 'An error occurred while uploading.'
        );
      }
    });
  }

  // 3. Save Social Links
  saveSocialLinks(): void {
    if (this.isSavingSocial()) return;

    this.isSavingSocial.set(true);
    this.profilesService.updateSocialLinks({
      faceBookUrl: this.facebookUrl().trim(),
      linkedInUrl: this.linkedInUrl().trim(),
      gitHubUrl: this.gitHubUrl().trim(),
      xUrl: this.xUrl().trim(),
      website: this.website().trim()
    }).subscribe({
      next: res => {
        this.isSavingSocial.set(false);
        if (res?.isSuccess) {
          this.toastService.success(
            this.translate.instant('Common.Success') || 'Success',
            this.translate.instant('ProfileEdit.Success.SocialUpdated') || 'Social links updated successfully.'
          );
          if (this.profile()) {
            this.profile.update(p => p ? {
              ...p,
              facebookUrl: this.facebookUrl().trim(),
              linkedInUrl: this.linkedInUrl().trim(),
              gitHubUrl: this.gitHubUrl().trim(),
              xUrl: this.xUrl().trim(),
              website: this.website().trim()
            } : null);
          }
        } else {
          this.toastService.error(
            this.translate.instant('Common.Error') || 'Error',
            this.translate.instant('ProfileEdit.Errors.UpdateFailed') || 'Failed to update social links.'
          );
        }
      },
      error: () => {
        this.isSavingSocial.set(false);
        this.toastService.error(
          this.translate.instant('Common.Error') || 'Error',
          this.translate.instant('ProfileEdit.Errors.UpdateFailed') || 'An error occurred while saving.'
        );
      }
    });
  }

  // 4. Skills Management
  openAddSkillModal(): void {
    this.isAddSkillModalOpen.set(true);
  }

  onSkillSelected(skill: ProjectSkillDto): void {
    this.profilesService.addSkillToProfile(skill.skillId, 1).subscribe({
      next: res => {
        if (res?.isSuccess) {
          const newSkill: ProfileSkillDto = {
            id: skill.skillId,
            skillId: skill.skillId,
            skillName: skill.skillName,
            proficiencyLevel: 1
          };
          this.skills.update(prev => [...prev, newSkill]);
          this.toastService.success(
            this.translate.instant('Common.Success') || 'Success',
            this.translate.instant('ProfileEdit.Success.SkillAdded') || 'Skill added successfully.'
          );
        } else {
          this.toastService.error(
            this.translate.instant('Common.Error') || 'Error',
            this.translate.instant('ProfileEdit.Errors.UpdateFailed') || 'Failed to add skill.'
          );
        }
      },
      error: () => {
        this.toastService.error(
          this.translate.instant('Common.Error') || 'Error',
          this.translate.instant('ProfileEdit.Errors.UpdateFailed') || 'An error occurred while adding the skill.'
        );
      }
    });
  }

  removeSkill(skill: ProfileSkillDto): void {
    if (this.removingSkillId()) return;

    this.removingSkillId.set(skill.skillId);
    this.profilesService.removeSkillFromProfile(skill.skillId).subscribe({
      next: res => {
        this.removingSkillId.set(null);
        if (res?.isSuccess) {
          this.skills.update(prev => prev.filter(s => s.skillId !== skill.skillId));
          this.toastService.success(
            this.translate.instant('Common.Success') || 'Success',
            this.translate.instant('ProfileEdit.Success.SkillRemoved') || 'Skill removed successfully.'
          );
        } else {
          this.toastService.error(
            this.translate.instant('Common.Error') || 'Error',
            this.translate.instant('ProfileEdit.Errors.UpdateFailed') || 'Failed to remove skill.'
          );
        }
      },
      error: () => {
        this.removingSkillId.set(null);
        this.toastService.error(
          this.translate.instant('Common.Error') || 'Error',
          this.translate.instant('ProfileEdit.Errors.UpdateFailed') || 'An error occurred while removing the skill.'
        );
      }
    });
  }
}

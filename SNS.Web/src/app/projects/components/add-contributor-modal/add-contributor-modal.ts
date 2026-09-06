import { Component, Input, OnInit, inject, signal, output, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { LucideX, LucideSearch, LucideCheck, LucideSparkles, LucideSend } from '@lucide/angular';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { ProjectContributorsService } from '../../services/project-contributors.service';
import { ProfileInvitationCandidateDto } from '../../contracts/profile-invitation-candidate.dto';
import { ProjectRole } from '../../enums/project-role.enum';
import { CircleLoader } from '../../../shared/Loading/components/circle-loader/circle-loader';
import { ToastService } from '../../../identity/notifications/services/toast.service';

@Component({
  selector: 'app-add-contributor-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    CircleLoader,
    LucideX,
    LucideSearch,
    LucideCheck,
    LucideSparkles,
    LucideSend
  ],
  templateUrl: './add-contributor-modal.html',
  styleUrl: './add-contributor-modal.css'
})
export class AddContributorModal implements OnInit {
  @Input({ required: true }) projectId!: string;

  readonly contributorInvited = output<void>();
  readonly closeModal = output<void>();

  private contributorsService = inject(ProjectContributorsService);
  private toastService = inject(ToastService);
  private translate = inject(TranslateService);

  readonly defaultAvatar = 'assets/images/default-avatar.png';
  readonly ProjectRole = ProjectRole;
  readonly availableRoles = Object.values(ProjectRole);

  searchQuery = signal<string>('');
  candidates = signal<ProfileInvitationCandidateDto[]>([]);
  selectedCandidate = signal<ProfileInvitationCandidateDto | null>(null);
  selectedRole = signal<ProjectRole>(ProjectRole.Developer);
  invitationMessage = signal<string>('');
  isLoadingCandidates = signal<boolean>(false);
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
      switchMap(search => {
        this.isLoadingCandidates.set(true);
        return this.contributorsService.getProfilesForProjectInvitation(this.projectId, search);
      })
    ).subscribe({
      next: res => {
        this.isLoadingCandidates.set(false);
        if (res?.isSuccess && res.value) {
          this.candidates.set(res.value);
        } else {
          this.candidates.set([]);
        }
      },
      error: () => {
        this.isLoadingCandidates.set(false);
        this.candidates.set([]);
      }
    });

    // Initial fetch of eligible candidates
    this.fetchCandidates('');
  }

  fetchCandidates(search = ''): void {
    this.searchQuery.set(search);
    this.searchSubject.next(search);
  }

  onSearchInput(val: string): void {
    this.searchQuery.set(val);
    this.searchSubject.next(val);
  }

  selectCandidate(candidate: ProfileInvitationCandidateDto): void {
    this.selectedCandidate.set(candidate);
    this.errorMessage.set(null);
  }

  onAvatarError(event: Event): void {
    const target = event.target as HTMLImageElement;
    if (target && target.src !== this.defaultAvatar) {
      target.src = this.defaultAvatar;
    }
  }

  sendInvitation(): void {
    const candidate = this.selectedCandidate();
    if (!candidate || !this.projectId || this.isSubmitting()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.contributorsService.addProjectContributor(this.projectId, {
      projectId: this.projectId,
      targetProfileId: candidate.id,
      role: this.selectedRole(),
      invitationMessage: this.invitationMessage().trim()
    }).subscribe({
      next: res => {
        this.isSubmitting.set(false);
        if (res?.isSuccess) {
          const successTitle = this.translate.instant('ProjectEdit.Contributors.Invitation_Sent_Title') || 'Invitation Sent';
          const successMsg = this.translate.instant('ProjectEdit.Contributors.Invitation_Sent_Message') || 'The contributor invitation has been sent successfully.';
          this.toastService.success(successTitle, successMsg);
          this.contributorInvited.emit();
          this.onClose();
        } else {
          this.errorMessage.set('Failed to send contributor invitation.');
        }
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('An error occurred while sending the invitation.');
      }
    });
  }

  onClose(): void {
    this.closeModal.emit();
  }
}

import {
  Component,
  computed,
  inject,
  signal,
  ElementRef,
  ViewChild
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { CreateProfileRequest } from '../../contracts/create-profile-request.dto';
import { ProfilesService } from '../../services/profiles.service';
import { finalize } from 'rxjs';
import { LoadingOnboardingService } from '../../../../shared/components/layouts/onboarding-layout/loading-onboarding.service';
import { HttpErrorResponse } from '@angular/common/http';
import { Result } from '../../../../shared/contracts/result';

/** All common software / engineering specializations. Replacing with an API
 *  call later only requires swapping the signal population in ngOnInit. */
const ALL_SPECIALIZATIONS: string[] = [
  'Software Engineer',
  'Backend Developer',
  'Frontend Developer',
  'Full Stack Developer',
  'Mobile Developer',
  'Android Developer',
  'iOS Developer',
  'Flutter Developer',
  '.NET Developer',
  'Java Developer',
  'Python Developer',
  'PHP Developer',
  'DevOps Engineer',
  'Cloud Engineer',
  'Site Reliability Engineer',
  'Data Engineer',
  'Data Scientist',
  'Machine Learning Engineer',
  'AI Engineer',
  'Cybersecurity Engineer',
  'QA Engineer',
  'Automation Tester',
  'UI Designer',
  'UX Designer',
  'Product Designer',
  'Game Developer',
  'Embedded Systems Engineer',
  'IoT Engineer',
  'Network Engineer',
  'System Administrator',
  'Database Administrator',
  'Solutions Architect',
  'Software Architect',
  'Electronics Engineer',
  'Electrical Engineer',
  'Mechanical Engineer',
  'Civil Engineer',
  'Industrial Engineer'
];

@Component({
  selector: 'app-create-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe],
  templateUrl: './create-profile.html',
  styleUrl: './create-profile.css'
})
export class CreateProfile {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private profileService = inject(ProfilesService);
  private loadingService = inject(LoadingOnboardingService);

  @ViewChild('fileInput') fileInputRef!: ElementRef<HTMLInputElement>;

  // ── Image state ────────────────────────────────────────────────────
  selectedImageFile = signal<File | null>(null);
  imagePreviewUrl = signal<string | null>(null);

  // ── Specialization search ──────────────────────────────────────────
  readonly specializationList = signal<string[]>(ALL_SPECIALIZATIONS);
  searchQuery = signal('');
  isDropdownOpen = signal(false);
  selectedSpecialization = signal<string | null>(null);

  filteredSpecializations = computed(() => {
    const q = this.searchQuery().toLowerCase().trim();
    if (!q) return this.specializationList();
    return this.specializationList().filter(s =>
      s.toLowerCase().includes(q)
    );
  });

  // ── Form ──────────────────────────────────────────────────────────
  readonly BIO_MAX = 300;
  readonly NAME_MAX = 100;

  profileForm: FormGroup = this.fb.group({
    fullName: ['', [Validators.required, Validators.maxLength(this.NAME_MAX)]],
    bio: ['', [Validators.maxLength(this.BIO_MAX)]]
  });

  isFormValid = computed(() => this.profileForm.valid);

  get bioLength(): number {
    return (this.profileForm.get('bio')?.value ?? '').length;
  }

  // ── Image handlers ─────────────────────────────────────────────────
  triggerFileInput(): void {
    this.fileInputRef.nativeElement.click();
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.selectedImageFile.set(file);

    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreviewUrl.set(reader.result as string);
    };
    reader.readAsDataURL(file);
  }

  removeImage(): void {
    this.selectedImageFile.set(null);
    this.imagePreviewUrl.set(null);
    this.fileInputRef.nativeElement.value = '';
  }

  // ── Specialization handlers ────────────────────────────────────────
  openDropdown(): void {
    this.isDropdownOpen.set(true);
  }

  closeDropdown(): void {
    // Slight delay to allow click-on-option to register before hiding
    setTimeout(() => this.isDropdownOpen.set(false), 150);
  }

  selectSpecialization(value: string): void {
    this.selectedSpecialization.set(value);
    this.searchQuery.set(value);
    this.isDropdownOpen.set(false);
  }

  clearSpecialization(): void {
    this.selectedSpecialization.set(null);
    this.searchQuery.set('');
    this.isDropdownOpen.set(false);
  }

  onSearchInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchQuery.set(value);
    // If user edits text manually, deselect the previously chosen option
    if (this.selectedSpecialization() && value !== this.selectedSpecialization()) {
      this.selectedSpecialization.set(null);
    }
    this.isDropdownOpen.set(true);
  }

  // ── Submit ─────────────────────────────────────────────────────────
  onSubmit(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    const fromData = this.profileForm.value;

    var profileCreateRequest: CreateProfileRequest = {
      profilePicture: this.selectedImageFile(),
      fullName: fromData.fullName,
      bio: fromData.bio,
      specialization: this.selectedSpecialization(),
    }

    this.loadingService.show();
    this.profileService.createProfile(profileCreateRequest)
      .pipe(
        finalize(() => {
          this.loadingService.hide();
        })
      )
      .subscribe({
        next: () => {
          this.router.navigate(['/onboarding/follow-people']);
        }
      });
  }
}

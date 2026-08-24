import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageService } from '../../../../../../shared/services/language.service';
import { UserManagementService } from '../../services/user-management.service';
import { SupportedLanguage } from '../../../../../../shared/contracts/supported-language.enum';
import { CommonModule } from '@angular/common';
import { tap, switchMap } from 'rxjs';
import { AppSelect } from '../../../../../../shared/design-system/components/app-select/app-select';
import { StateControllerService } from '../../../../shared/services/state-controller.service';

@Component({
  selector: 'app-change-language',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TranslatePipe,
    ReactiveFormsModule,
    AppSelect
  ],
  templateUrl: './change-language.html',
  styleUrl: './change-language.css'
})
export class ChangeLanguage {
  private fb = inject(FormBuilder);
  private languageService = inject(LanguageService);
  private userManagementService = inject(UserManagementService);
  private stateControllerService = inject(StateControllerService);

  isSubmitting = signal(false);
  supportedLanguage = SupportedLanguage;

  form: FormGroup;

  constructor() {
    this.form = this.fb.group({
      preferredLanguage: [this.languageService.currentLanguage()]
    });
  }

  get selectedLanguage(): SupportedLanguage {
    return Number(this.form.value.preferredLanguage) as SupportedLanguage;
  }

  get hasChanged(): boolean {
    return this.selectedLanguage !== this.languageService.currentLanguage();
  }

  onSubmit() {
    if (!this.hasChanged || this.isSubmitting()) {
      return;
    }

    const key = 'Identity.Security_Settings.Personal_Info.Language.Changing';

    this.stateControllerService.start(key);

    const newLang = this.selectedLanguage;

    this.userManagementService.changePreferredLanguage(newLang).pipe(

      tap(result => {
        if (!result.isSuccess) {
          throw new Error();
        }

        this.stateControllerService.run();
      }),

      switchMap(() => this.languageService.setLanguage(newLang))

    ).subscribe({
      next: () => {
        setTimeout(() => {
          this.stateControllerService.stop();
        }, 2000);

        setTimeout(() => {
          this.stateControllerService.ready(key);
        }, 2500);
      },
      error: () => {
        this.stateControllerService.ready(key);
      }
    });
  }
}

import { Component, inject, OnInit, OnDestroy, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { catchError, debounceTime, distinctUntilChanged, filter, finalize, single, switchMap, tap } from 'rxjs/operators';
import { of, Subscription } from 'rxjs';
import { UserManagementService } from '../../services/user-management.service';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { LucideCheckLine, LucideCircleCheckBig, LucideCircleX, LucideChevronLeft, LucideMoveLeft } from "@lucide/angular";
import { Result } from '../../../../../shared/contracts/result';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from "@angular/router";
import { ToastService } from '../../../../notifications/services/toast.service';
import { LoadingSettingsService } from '../../../../shared/layout/services/loading-settings.service';

@Component({
  selector: 'app-change-username',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    TranslatePipe,
    LucideCheckLine,
    LucideCircleCheckBig,
    LucideCircleX,
    LucideChevronLeft,
    RouterLink,
    LucideMoveLeft
],
  templateUrl: './change-username.html',
  styleUrl: './change-username.css',
})
export class ChangeUsername implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);
  private userService = inject(UserManagementService);
  private translateService = inject(TranslateService);
  private loadingService = inject(LoadingSettingsService);

  form: FormGroup = this.fb.group({
    username: [history.state.username, [Validators.required, Validators.minLength(3)]]
  });

  isLoadingResponse = signal(false);
  isAvailable = signal<boolean | null>(null);
  isLoadingChangingResult = this.loadingService.isLoadingSettings;
  isLoadingAvalibilityResult = signal(false);
  currentUsername = history.state.username;
  enteredUsername = history.state.username;
  private sub: Subscription = new Subscription();

  ngOnInit() {
    this.sub = this.form.get('username')!.valueChanges
      .pipe(
        debounceTime(600),
        distinctUntilChanged(),
        tap((val) => {
          this.enteredUsername = val;
          if (!val || val.length < 3) {
            this.isLoadingAvalibilityResult.set(false);
            this.isAvailable.set(null);
          } else {
            this.isLoadingAvalibilityResult.set(true);
            this.isAvailable.set(null);
          }
        }),
        filter(username =>
          !!username &&
          username.length >= 3 &&
          username !== this.currentUsername
        ),

        switchMap(username => {
          if (!username || username.length < 3) {
            return of(null);
          }
          return this.userService.checkUsernameAvailabilty(username).pipe(
            catchError(() => of({ value: false }))
          );
        }),
        catchError(() => of({ value: false }))
      )
      .subscribe((result: any) => {
        this.isLoadingAvalibilityResult.set(false);
        this.isAvailable.set(result?.value ?? false);
      });
  }

  onSubmit() {
    if (this.form.valid && this.isAvailable()) {
      this.loadingService.show();
      const inputedUsername = this.form.value.username;
      this.userService.changeUserName(inputedUsername)
        .subscribe({
          next: (res: Result<string>) => {
            this.loadingService.hide();
            this.toast.success(
              this.translateService.instant("Status_Codes.Titles.Success"),
              this.translateService.instant(`Status_Codes.${res.statusCode.category}.${res.statusCode.code}`)
            );
            this.currentUsername = inputedUsername;
          },
          error: (err: HttpErrorResponse) => {
            this.loadingService.hide();
          }
        });
    }
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
import { Component, inject, OnInit, OnDestroy, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { catchError, debounceTime, distinctUntilChanged, filter, single, switchMap, tap } from 'rxjs/operators';
import { of, Subscription } from 'rxjs';
import { UserManagementService } from '../../../users/user-management/services/user-management.service';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { LucideAArrowDown, LucideCheckLine, LucideCircleCheckBig, LucideCircleX, LucideChevronLeft } from "@lucide/angular";
import { LinearLoaderComponent } from "../../../../shared/components/loaders/linear-loader/linear-loader.component";
import { SimpleCircleLoaderComponent } from "../../../../shared/components/loaders/simple-circle-loader/simple-circle-loader.component";
import { Result } from '../../../../shared/contracts/result';
import { ToastService } from '../../../../shared/services/toast.service';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-change-username',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    TranslatePipe,
    LucideCheckLine,
    LinearLoaderComponent,
    LucideCircleCheckBig,
    LucideCircleX,
    LucideChevronLeft,
    RouterLink
],
  templateUrl: './change-username.component.html',
  styleUrl: './change-username.component.css',
})
export class ChangeUsernameComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);
  private userService = inject(UserManagementService);
  private translateService = inject(TranslateService);

  form: FormGroup = this.fb.group({
    username: [history.state.username, [Validators.required, Validators.minLength(3)]]
  });

  isLoadingResponse = signal(false);
  isAvailable = signal<boolean | null>(null);
  isLoading = signal(false);
  currentUsername = history.state.username;
  enteredUsername = history.state.username;
  private sub: Subscription = new Subscription();

  ngOnInit() {
    this.sub = this.form.get('username')!.valueChanges
      .pipe(
        debounceTime(600),
        distinctUntilChanged(),
        tap((val) => {
          console.log("isLoading: ", this.isLoading);
          console.log("isAvailable: ", this.isAvailable);
          this.enteredUsername = val;
          if (!val || val.length < 3) {
            this.isLoading.set(false);
            this.isAvailable.set(null);
          } else {
            this.isLoading.set(true);
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
        console.log("isLoading: ", this.isLoading);
        console.log("result: ", result);

        this.isLoading.set(false);
        this.isAvailable.set(result?.value ?? false);
      });
  }

  onSubmit() {
    if (this.form.valid && this.isAvailable()) {
      this.isLoadingResponse.set(true);
      this.userService.changeUserName(this.form.value.username)
        .subscribe({
          next: (res: Result<string>) => {
            this.isLoadingResponse.set(false);
            this.toast.success(
              this.translateService.instant("Status_Codes.Titles.Success"),
              this.translateService.instant(`Status_Codes.${res.statusCode.category}.${res.statusCode.code}`)
            );
          },
          error: (err: HttpErrorResponse) => {
            this.isLoadingResponse.set(false);
            var result = err.error as Result<string>;
            this.toast.error(
              this.translateService.instant(`Status_Codes.Titles.Error`),
              this.translateService.instant(`Status_Codes.${result.statusCode.category}.${result.statusCode.code}`)
            );
          }
        });
    }
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
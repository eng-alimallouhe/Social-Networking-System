import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LineLoader } from '../../loaders/line-loader/line-loader';
import { LoadingOnboardingService } from './loading-onboarding.service';

@Component({
  selector: 'app-onboarding-layout',
  standalone: true,
  imports: [RouterOutlet, LineLoader],
  templateUrl: './onboarding-layout.html',
  styleUrl: './onboarding-layout.css',
})
export class OnboardingLayout {
  private loadingService = inject(LoadingOnboardingService);

  isLoading = this.loadingService.isLoading;
}

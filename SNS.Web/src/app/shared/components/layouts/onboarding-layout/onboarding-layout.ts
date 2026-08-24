import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LineLoader } from '../../../Loading/components/line-loader/line-loader';
import { LoadingOnboardingService } from './loading-onboarding.service';
import { GlobalLoaderService } from '../../../Loading/services/global-loader.service';

@Component({
  selector: 'app-onboarding-layout',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './onboarding-layout.html',
  styleUrl: './onboarding-layout.css',
})
export class OnboardingLayout {
}

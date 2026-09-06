import { Component, Input, Output, EventEmitter, signal, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { AppInput } from '../../../shared/design-system/components/app-input/app-input';
import { ModerationService } from '../../services/moderation.service';
import { ViolationReason } from '../../enums/violation-reason.enum';
import { LucideX, LucideBan, LucideOctagonAlert, LucideBadgeCheck } from '@lucide/angular';
import { PageService } from '../../../shared/services/page.service';

@Component({
  selector: 'app-job-report',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    AppInput,
    LucideX,
    LucideBan,
    LucideOctagonAlert,
    LucideBadgeCheck
  ],
  templateUrl: './job-report.html',
  styleUrl: './job-report.css'
})
export class JobReport {
  @Input() jobId!: string;
  @Input() isOpen = signal(false);
  @Output() closed = new EventEmitter<void>();

  private moderationService = inject(ModerationService);
  private pageService = inject(PageService);

  currentStep = signal(1);
  selectedReason = signal<ViolationReason | null>(null);
  additionalDetails = signal<string>('');
  isSubmitting = signal(false);

  reasons = [
    {
      value: ViolationReason.Spam,
      titleKey: 'App.Moderation.PostReport.Spam',
      descKey: 'App.Moderation.PostReport.SpamDesc',
      icon: 'ban'
    },
    {
      value: ViolationReason.HateSpeech,
      titleKey: 'App.Moderation.PostReport.HateSpeech',
      descKey: 'App.Moderation.PostReport.HateSpeechDesc',
      icon: 'alert-triangle'
    },
    {
      value: ViolationReason.Harassment,
      titleKey: 'App.Moderation.PostReport.Harassment',
      descKey: 'App.Moderation.PostReport.HarassmentDesc',
      icon: 'alert-triangle'
    },
    {
      value: ViolationReason.Misinformation,
      titleKey: 'App.Moderation.PostReport.Misinformation',
      descKey: 'App.Moderation.PostReport.MisinformationDesc',
      icon: 'alert-triangle'
    },
    {
      value: ViolationReason.Other,
      titleKey: 'App.Moderation.PostReport.Other',
      descKey: 'App.Moderation.PostReport.OtherDesc',
      icon: 'alert-triangle'
    }
  ];

  constructor() {
    effect(() => {
      if (this.isOpen()) {
        this.pageService.disableScroll();
      } else {
        this.pageService.enableScroll();
      }
    });
  }

  selectReason(reason: ViolationReason) {
    this.selectedReason.set(reason);
  }

  onContinue() {
    if (this.selectedReason()) {
      this.currentStep.set(2);
    }
  }

  onBack() {
    this.currentStep.set(1);
  }

  onClose() {
    if (!this.isSubmitting()) {
      this.isOpen.set(false);
      this.closed.emit();
      this.reset();
    }
  }

  onSubmit() {
    const reason = this.selectedReason();
    if (!reason || this.isSubmitting()) return;

    this.isSubmitting.set(true);

    this.moderationService.reportJob(this.jobId, {
      violationReason: reason,
      additionalDetails: this.additionalDetails() || null
    }).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.currentStep.set(3);
      },
      error: () => {
        this.isSubmitting.set(false);
      }
    });
  }

  onDone() {
    this.isOpen.set(false);
    this.closed.emit();
    this.reset();
  }

  private reset() {
    this.currentStep.set(1);
    this.selectedReason.set(null);
    this.additionalDetails.set('');
  }
}

// toast-item.component.ts
import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Toast } from '../../../services/toast.service';
import { LucideCheckCircle2, LucideXCircle, LucideAlertTriangle, LucideInfo, LucideX, LucideBan, LucideBadgeCheck, LucideOctagonAlert, LucideCircleX } from "@lucide/angular";

@Component({
  selector: 'app-toast-item',
  standalone: true,
  styleUrl: './toast-item.component.css',
  imports: [
    CommonModule,
    LucideInfo,
    LucideX,
    LucideCircleX,
    LucideBadgeCheck,
    LucideOctagonAlert
],
  template: `
    <div 
      class="toast" 
      [ngClass]="toast.type"
      [class.closing]="isClosing"
      (mouseenter)="onMouseEnter()" 
      (mouseleave)="onMouseLeave()">
      
      <div class="toast-icon">
        @switch (toast.type) {
          @case ('success') { <svg lucideBadgeCheck></svg> }
          @case ('error') { <svg lucideCircleX></svg> }
          @case ('warning') { <svg lucideOctagonAlert></svg> }
          @case ('info') { <svg lucideInfo></svg> }
        }
      </div>
      
      <div class="toast-content">
        <div class="toast-title">{{ toast.title }}</div>
        <div class="toast-message">{{ toast.message }}</div>
      </div>
      
      <button class="toast-close" (click)="close()">
        <svg lucideX></svg>
      </button>
      
      <div class="progress-track">
        <div 
          class="progress-bar" 
          [style.animation-duration.ms]="toast.duration"
          [style.animation-play-state]="isHovered ? 'paused' : 'running'">
        </div>
      </div>
    </div>
  `
})
export class ToastItemComponent implements OnInit, OnDestroy {
  @Input({ required: true }) toast!: Toast;
  @Output() remove = new EventEmitter<string>();

  isClosing = false;
  isHovered = false;
  private timeoutId: any;
  private remainingTime!: number;
  private startTime!: number;

  ngOnInit() {
    this.remainingTime = this.toast.duration;
    this.startTimer();
  }

  startTimer() {
    this.startTime = Date.now();
    this.timeoutId = setTimeout(() => {
      this.close();
    }, this.remainingTime);
  }

  onMouseEnter() {
    this.isHovered = true;
    clearTimeout(this.timeoutId);
    // حساب الوقت المتبقي لكي نكمله بعد إزالة الماوس
    this.remainingTime -= (Date.now() - this.startTime);
  }

  onMouseLeave() {
    this.isHovered = false;
    this.startTimer();
  }

  close() {
    // تشغيل أنيميشن الإغلاق أولاً
    this.isClosing = true;
    // الانتظار حتى ينتهي الأنيميشن (0.4s) ثم حذفه من الخدمة
    setTimeout(() => {
      this.remove.emit(this.toast.id);
    }, 400); 
  }

  ngOnDestroy() {
    clearTimeout(this.timeoutId);
  }
}
import { Directive, ElementRef, Input, OnInit, OnDestroy, inject } from '@angular/core';
import { ViewTrackingService } from '../../content/posts/services/post-view-tracking.service';

@Directive({
  selector: '[appTrackVisibility]',
  standalone: true
})
export class TrackVisibilityDirective implements OnInit, OnDestroy {
  @Input('appTrackVisibility') postId!: string;

  private element = inject(ElementRef);
  private viewTrackingService = inject(ViewTrackingService);
  
  private observer!: IntersectionObserver;
  private viewTimeout: any;
  private initDelayTimeout: any;

  ngOnInit() {
    // 1. إذا تمت مشاهدته سابقاً، لا تقم حتى بإنشاء المراقب! (توفير هائل للذاكرة)
    if (this.viewTrackingService.isAlreadyTracked(this.postId)) {
      return; 
    }

    const options = {
      root: null,
      threshold: 0.5 
    };

    this.observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        // 2. أهم خطوة: امسح أي مؤقت سابق فوراً عند أي حركة سكرول!
        clearTimeout(this.viewTimeout);

        if (entry.isIntersecting) {
          // دخل الشاشة: نبدأ العداد 3 ثواني
          this.viewTimeout = setTimeout(() => {
            console.log(`✅ ${this.postId}: is viewed`);
            this.viewTrackingService.trackView(this.postId);
            this.observer.disconnect(); // نوقف المراقبة للأبد
          }, 3000);
        }
      });
    }, options);

    // 3. تأخير بدء المراقبة ثانية واحدة لكي نتأكد أن شاشة التحميل (Overlay) قد اختفت
    this.initDelayTimeout = setTimeout(() => {
      if (this.element.nativeElement) {
        this.observer.observe(this.element.nativeElement);
      }
    }, 1500); 
  }

  ngOnDestroy() {
    if (this.observer) {
      this.observer.disconnect();
    }
    clearTimeout(this.viewTimeout);
    clearTimeout(this.initDelayTimeout);
  }
}
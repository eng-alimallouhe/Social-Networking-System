import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';
import { Result } from '../../../shared/dtos/result.dto';

@Injectable({ providedIn: 'root' })
export class ViewTrackingService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl + 'posts/views'; // تأكد من المسار الصحيح
  
  // 1. سلة الذاكرة الدائمة (لكي لا نحسب مشاهدة البوست مرتين أبداً في نفس الجلسة)
  private sessionViewedIds = new Set<string>(); 

  // 2. سلة الإرسال المؤقتة (لإرسالها كل 15 ثانية)
  private pendingBatchIds = new Set<string>(); 

  constructor() {
    setInterval(() => this.flushViews(), 15000);
  }

  // دالة ليتحقق منها المكون قبل بدء المراقبة
  isAlreadyTracked(postId: string): boolean {
    return this.sessionViewedIds.has(postId);
  }

  trackView(postId: string) {
    if (this.sessionViewedIds.has(postId)) return; 

    this.sessionViewedIds.add(postId);
    this.pendingBatchIds.add(postId);
  }

  private flushViews() {
    if (this.pendingBatchIds.size === 0) return;

    const idsToSend = Array.from(this.pendingBatchIds);
    this.pendingBatchIds.clear();

    console.log('🚀 Sending batch views to server:', idsToSend);
    
    this.http.post<Result<void>>(`${this.apiUrl}/batch`, { postIds: idsToSend }).subscribe();
  }
}
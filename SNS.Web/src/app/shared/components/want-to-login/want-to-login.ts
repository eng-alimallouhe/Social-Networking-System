import { Component, Output, EventEmitter, model, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { LucideX, LucideLogIn } from '@lucide/angular';
import { PageService } from '../../services/page.service';

@Component({
  selector: 'app-want-to-login',
  standalone: true,
  imports: [CommonModule, TranslatePipe, LucideX, LucideLogIn],
  templateUrl: './want-to-login.html',
  styleUrls: ['./want-to-login.css']
})
export class WantToLogin {
  isOpen = model<boolean>(false);
  @Output() closed = new EventEmitter<void>();
  private pageService = inject(PageService);

  constructor(private router: Router) {
    effect(() => {
      if (this.isOpen()) {
        this.pageService.disableScroll();
      } else {
        this.pageService.enableScroll();
      }
    })
  }

  onClose() {
    this.isOpen.set(false);
    this.closed.emit();
  }

  onLogin() {
    this.isOpen.set(false);
    this.closed.emit();
    this.router.navigate(['/auth/login']);
  }
}

import {
  afterNextRender,
  Component,
  ElementRef,
  HostListener,
  OnDestroy,
  signal,
  ViewChild,
  effect,
  Inject,
  inject,
  NgZone
} from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { RouterOutlet, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize, forkJoin } from 'rxjs';
import { GlobalLoaderService } from '../../../shared/Loading/services/global-loader.service';
import { SessionManagementService } from '../../../identity/account-settings/security-sessions/session-management/services/session-management.service';
import { AuthenticationService } from '../../../identity/shared/services/authentication.service';

@Component({
  selector: 'app-demo-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    TranslatePipe
  ],
  templateUrl: './demo-layout.html',
  styleUrl: './demo-layout.css',
})
export class DemoLayout implements OnDestroy {
  private router = inject(Router);
  private authenticationService = inject(AuthenticationService);
  private translateService = inject(TranslateService);
  private loadingService = inject(GlobalLoaderService);
  private sessionManagementService = inject(SessionManagementService);
  private ngZone = inject(NgZone);

  @ViewChild('particleCanvas') canvasRef!: ElementRef<HTMLCanvasElement>;

  isLoading = this.loadingService.isLoading;

  private ctx!: CanvasRenderingContext2D;
  private particles: Particle[] = [];
  private mouse = { x: -1000, y: -1000, radius: 150 };

  private animationFrameId?: number;
  private resizeTimeout?: ReturnType<typeof setTimeout>;
  private mouseMoveCleanup?: () => void;

  isOptionsMenuOpen = signal(false);

  currentUrl = signal<string | null>(null);
  isAuthenticated = this.authenticationService.isAuthenticated;
  isDarkMode = signal<boolean>(false);
  showControls = false;

  constructor(@Inject(DOCUMENT) private document: Document) {
    afterNextRender(() => {
      const savedTheme = localStorage.getItem('theme');
      if (savedTheme === 'dark') {
        this.isDarkMode.set(true);
      }

      // تشغيل الكانفاس، الأنيميشن، وتتبع الماوس خارج نطاق Zone.js بالكامل
      this.ngZone.runOutsideAngular(() => {
        this.initCanvas();
        this.animate();

        const onMove = (event: MouseEvent) => {
          this.mouse.x = event.clientX;
          this.mouse.y = event.clientY;
        };

        window.addEventListener('mousemove', onMove, { passive: true });
        this.mouseMoveCleanup = () => window.removeEventListener('mousemove', onMove);
      });
    });

    effect(() => {
      const isDark = this.isDarkMode();
      if (isDark) {
        this.document.body.classList.add('dark-mode');
      } else {
        this.document.body.classList.remove('dark-mode');
      }
    });
  }

  toggleOptionsMenu() {
    this.isOptionsMenuOpen.update(value => !value);
  }

  ngOnDestroy() {
    // إلغاء مستمع حركة الماوس
    if (this.mouseMoveCleanup) {
      this.mouseMoveCleanup();
    }
    // تنظيف الأنيميشن لمنع تسرب الذاكرة
    if (this.animationFrameId) {
      cancelAnimationFrame(this.animationFrameId);
    }
    if (this.resizeTimeout) {
      clearTimeout(this.resizeTimeout);
    }
  }

  logout() {
    this.loadingService.show();
    this.sessionManagementService.logout()
      .pipe(finalize(() => {
        this.loadingService.hide();
      }))
      .subscribe();
  }

  toggleControls() {
    this.showControls = !this.showControls;
  }

  toggleTheme() {
    const newMode = !this.isDarkMode();
    this.isDarkMode.set(newMode);
    localStorage.setItem('theme', newMode ? 'dark' : 'light');
  }

  @HostListener('window:resize')
  onResize() {
    if (this.resizeTimeout) {
      clearTimeout(this.resizeTimeout);
    }
    this.resizeTimeout = setTimeout(() => {
      this.ngZone.runOutsideAngular(() => {
        this.initCanvas();
      });
    }, 200);
  }

  navigateTo(to: string) {
    if (this.isLoading()) {
      forkJoin({
        message: this.translateService.get(`Loader.System_Busy_Body`),
        title: this.translateService.get(`Loader.System_Busy_Title`)
      }).subscribe();
      return;
    }
    if (to === 'login-options') {
      this.router.navigate(['/auth/login-options']);
    } else if (to === 'support') {
      this.router.navigate(['/support']);
    }
  }

  goToRoleSwitcher() {
    this.router.navigate(['/demo/role-switcher']);
  }

  private initCanvas() {
    if (!this.canvasRef) return;

    const canvas = this.canvasRef.nativeElement;
    this.ctx = canvas.getContext('2d')!;
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;

    this.particles = [];
    const numberOfParticles = Math.floor((canvas.width * canvas.height) / 12000);
    for (let i = 0; i < numberOfParticles; i++) {
      this.particles.push(new Particle(canvas, this.ctx));
    }
  }

  private animate() {
    if (!this.ctx) return;

    this.ctx.clearRect(0, 0, this.canvasRef.nativeElement.width, this.canvasRef.nativeElement.height);

    const isDark = this.isDarkMode();
    for (const particle of this.particles) {
      particle.update(this.mouse);
      particle.draw(isDark);
    }
    this.connect(isDark);

    // سيعمل الآن بسلاسة خارج Zone.js دون إطلاق Change Detection
    this.animationFrameId = requestAnimationFrame(() => this.animate());
  }

  private connect(isDark: boolean) {
    const opacity = isDark ? 0.15 : 0.1;
    const color = isDark ? `rgba(122, 186, 214, ${opacity})` : `rgba(47, 102, 133, ${opacity})`;

    this.ctx.strokeStyle = color;
    this.ctx.lineWidth = 1;

    for (let a = 0; a < this.particles.length; a++) {
      for (let b = a + 1; b < this.particles.length; b++) {
        const dx = this.particles[a].x - this.particles[b].x;
        const dy = this.particles[a].y - this.particles[b].y;
        const distance = dx * dx + dy * dy;

        if (distance < 14400) {
          this.ctx.beginPath();
          this.ctx.moveTo(this.particles[a].x, this.particles[a].y);
          this.ctx.lineTo(this.particles[b].x, this.particles[b].y);
          this.ctx.stroke();
        }
      }
    }
  }

  isClicked = signal(false);

  onLinkClick(event: Event) {
    if (this.isLoading()) {
      event.preventDefault();
      this.isClicked.set(true);

      setTimeout(() => {
        this.isClicked.set(false);
      }, 2000);
    }
  }
}

class Particle {
  x: number;
  y: number;
  size: number;
  speedX: number;
  speedY: number;

  constructor(private canvas: HTMLCanvasElement, private ctx: CanvasRenderingContext2D) {
    this.x = Math.random() * canvas.width;
    this.y = Math.random() * canvas.height;
    this.size = Math.random() * 3 + 1.5;
    this.speedX = Math.random() * 0.8 - 0.4;
    this.speedY = Math.random() * 0.8 - 0.4;
  }

  update(mouse: { x: number; y: number; radius: number }) {
    this.x += this.speedX;
    this.y += this.speedY;

    if (this.x > this.canvas.width) this.x = 0;
    if (this.x < 0) this.x = this.canvas.width;
    if (this.y > this.canvas.height) this.y = 0;
    if (this.y < 0) this.y = this.canvas.height;

    const dx = mouse.x - this.x;
    const dy = mouse.y - this.y;
    const distance = dx * dx + dy * dy;

    if (distance < mouse.radius * mouse.radius) {
      const force = 2;
      if (mouse.x < this.x && this.x < this.canvas.width - this.size * 10) this.x += force;
      if (mouse.x > this.x && this.x > this.size * 10) this.x -= force;
      if (mouse.y < this.y && this.y < this.canvas.height - this.size * 10) this.y += force;
      if (mouse.y > this.y && this.y > this.size * 10) this.y -= force;
    }
  }

  draw(isDark: boolean) {
    this.ctx.fillStyle = isDark ? 'rgba(122, 186, 214, 0.7)' : 'rgba(47, 102, 133, 0.4)';
    this.ctx.beginPath();
    this.ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
    this.ctx.fill();
  }
}
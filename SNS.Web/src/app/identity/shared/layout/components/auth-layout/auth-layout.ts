import {
  afterNextRender,
  Component,
  ElementRef,
  HostListener,
  OnDestroy,
  signal,
  ViewChild,
  Inject,
  inject,
  computed,
  NgZone
} from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { RouterOutlet, Router } from '@angular/router';
import { LucideLifeBuoy, LucideFingerprint, LucideEllipsis } from '@lucide/angular';
import { ConnectedPosition, OverlayModule } from '@angular/cdk/overlay';
import { ToastService } from '../../../../notifications/services/toast.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { forkJoin } from 'rxjs';
import { GlobalLoaderService } from '../../../../../shared/Loading/services/global-loader.service';
import { Theme, ThemeChanger } from '../../../../../shared/services/theme-changer';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    LucideLifeBuoy,
    LucideFingerprint,
    LucideEllipsis,
    OverlayModule,
    TranslatePipe
  ],
  templateUrl: './auth-layout.html',
  styleUrl: './auth-layout.css',
})
export class AuthLayout implements OnDestroy {
  private loaderService = inject(GlobalLoaderService);
  private router = inject(Router);
  private toastService = inject(ToastService);
  private translateService = inject(TranslateService);
  private themeChanger = inject(ThemeChanger);
  private ngZone = inject(NgZone);

  @ViewChild('particleCanvas') canvasRef!: ElementRef<HTMLCanvasElement>;

  private ctx!: CanvasRenderingContext2D;
  private particles: Particle[] = [];
  private mouse = { x: -1000, y: -1000, radius: 150 };

  private animationFrameId?: number;
  private resizeTimeout?: ReturnType<typeof setTimeout>;
  private mouseMoveCleanup?: () => void;

  overlayPositions: ConnectedPosition[] = [
    {
      originX: 'center',
      originY: 'bottom',
      overlayX: 'center',
      overlayY: 'top',
      offsetY: 15,
      offsetX: -125
    }
  ];

  isOptionsMenuOpen = signal(false);
  isLoading = this.loaderService.isLoading;
  isDarkMode = computed(() => this.themeChanger.currentTheme() === Theme.Dark);
  showControls = false;
  isClicked = signal(false);

  constructor(@Inject(DOCUMENT) private document: Document) {
    afterNextRender(() => {
      // عزل كل عمليات الرسم وحركة الماوس تماماً عن أنجولار
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
  }

  toggleOptionsMenu() {
    this.isOptionsMenuOpen.update(value => !value);
  }

  ngOnDestroy() {
    if (this.mouseMoveCleanup) {
      this.mouseMoveCleanup();
    }
    if (this.animationFrameId) {
      cancelAnimationFrame(this.animationFrameId);
    }
    if (this.resizeTimeout) {
      clearTimeout(this.resizeTimeout);
    }
  }

  toggleControls() {
    this.showControls = !this.showControls;
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
      }).subscribe(translations => {
        this.toastService.error(translations.title, translations.message);
      });
      return;
    }

    if (to === 'login-options') {
      this.router.navigate(['/auth/login-options']);
    } else if (to === 'support') {
      this.router.navigate(['/support']);
    }
  }

  private initCanvas() {
    if (!this.canvasRef) return;

    const canvas = this.canvasRef.nativeElement;
    this.ctx = canvas.getContext('2d')!;

    const displayWidth = canvas.clientWidth;
    const displayHeight = canvas.clientHeight;

    const dpr = window.devicePixelRatio || 1;
    canvas.width = Math.floor(displayWidth * dpr);
    canvas.height = Math.floor(displayHeight * dpr);

    this.ctx.setTransform(1, 0, 0, 1, 0, 0); // إعادة تصفير التحويل لتجنب تراكم الـ scale عند الـ resize
    this.ctx.scale(dpr, dpr);

    this.particles = [];
    const numberOfParticles = Math.floor((displayWidth * displayHeight) / 12000);
    for (let i = 0; i < numberOfParticles; i++) {
      this.particles.push(new Particle(canvas, this.ctx));
    }
  }

  private animate() {
    if (!this.ctx || !this.canvasRef) return;

    const canvas = this.canvasRef.nativeElement;
    this.ctx.clearRect(0, 0, canvas.width, canvas.height);

    const isDark = this.isDarkMode();
    for (const particle of this.particles) {
      particle.update(this.mouse);
      particle.draw(isDark);
    }
    this.connect(isDark);

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
    this.x = Math.random() * (canvas.width / (window.devicePixelRatio || 1));
    this.y = Math.random() * (canvas.height / (window.devicePixelRatio || 1));
    this.size = Math.random() * 3 + 1.5;
    this.speedX = Math.random() * 0.8 - 0.4;
    this.speedY = Math.random() * 0.8 - 0.4;
  }

  update(mouse: { x: number; y: number; radius: number }) {
    const dpr = window.devicePixelRatio || 1;
    const boundWidth = this.canvas.width / dpr;
    const boundHeight = this.canvas.height / dpr;

    this.x += this.speedX;
    this.y += this.speedY;

    if (this.x > boundWidth) this.x = 0;
    if (this.x < 0) this.x = boundWidth;
    if (this.y > boundHeight) this.y = 0;
    if (this.y < 0) this.y = boundHeight;

    const dx = mouse.x - this.x;
    const dy = mouse.y - this.y;
    const distance = dx * dx + dy * dy;

    if (distance < mouse.radius * mouse.radius) {
      const force = 2;
      if (mouse.x < this.x && this.x < boundWidth - this.size * 10) this.x += force;
      if (mouse.x > this.x && this.x > this.size * 10) this.x -= force;
      if (mouse.y < this.y && this.y < boundHeight - this.size * 10) this.y += force;
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
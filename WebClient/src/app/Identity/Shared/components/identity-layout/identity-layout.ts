import { Component, ElementRef, HostListener, ViewChild, afterNextRender, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgIf } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-identity-layout',
  standalone: true,
  imports: [RouterOutlet, NgIf, TranslateModule],
  templateUrl: './identity-layout.html',
  styleUrl: './identity-layout.css'
})
export class IdentityLayout {
  @ViewChild('particleCanvas') canvasRef!: ElementRef<HTMLCanvasElement>;

  private ctx!: CanvasRenderingContext2D;
  private particles: Particle[] = [];
  private mouse = { x: 0, y: 0, radius: 150 };

  isDarkMode = signal(localStorage.getItem('theme') === 'dark');

  constructor() {
    if (this.isDarkMode()) {
      document.body.classList.add('dark-mode');
    }

    afterNextRender(() => {
      this.initCanvas();
      this.animate();
    });
  }

  showControls = false;

  toggleControls() {
    this.showControls = !this.showControls;
  }

  @HostListener('window:mousemove', ['$event'])
  onMouseMove(event: MouseEvent) {
    this.mouse.x = event.clientX;
    this.mouse.y = event.clientY;
  }

  @HostListener('window:resize')
  onResize() {
    this.initCanvas();
  }

  toggleTheme() {
    const newMode = !this.isDarkMode();
    this.isDarkMode.set(newMode);
    localStorage.setItem('theme', newMode ? 'dark' : 'light');
    document.body.classList.toggle('dark-mode', newMode);
  }

  private initCanvas() {
    const canvas = this.canvasRef.nativeElement;
    this.ctx = canvas.getContext('2d')!;
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;

    this.particles = [];
    const numberOfParticles = (canvas.width * canvas.height) / 11000;
    for (let i = 0; i < numberOfParticles; i++) {
      this.particles.push(new Particle(canvas, this.ctx));
    }
  }

  private animate() {
    this.ctx.clearRect(0, 0, this.canvasRef.nativeElement.width, this.canvasRef.nativeElement.height);
    for (const particle of this.particles) {
      particle.update(this.mouse);
      particle.draw(this.isDarkMode());
    }
    this.connect();
    requestAnimationFrame(() => this.animate());
  }

  private connect() {
    const opacity = this.isDarkMode() ? 0.15 : 0.1;
    const color = this.isDarkMode() ? `rgba(122, 186, 214, ${opacity})` : `rgba(47, 102, 133, ${opacity})`;

    for (let a = 0; a < this.particles.length; a++) {
      for (let b = a; b < this.particles.length; b++) {
        const dx = this.particles[a].x - this.particles[b].x;
        const dy = this.particles[a].y - this.particles[b].y;
        const distance = Math.sqrt(dx * dx + dy * dy);
        if (distance < 120) {
          this.ctx.strokeStyle = color;
          this.ctx.lineWidth = 1;
          this.ctx.beginPath();
          this.ctx.moveTo(this.particles[a].x, this.particles[a].y);
          this.ctx.lineTo(this.particles[b].x, this.particles[b].y);
          this.ctx.stroke();
        }
      }
    }
  }
}

class Particle {
  x: number; y: number; size: number; speedX: number; speedY: number;
  constructor(private canvas: HTMLCanvasElement, private ctx: CanvasRenderingContext2D) {
    this.x = Math.random() * canvas.width;
    this.y = Math.random() * canvas.height;
    this.size = Math.random() * 4 + 2;
    this.speedX = Math.random() * 0.8 - 0.4;
    this.speedY = Math.random() * 0.8 - 0.4;
  }
  update(mouse: any) {
    this.x += this.speedX; this.y += this.speedY;
    if (this.x > this.canvas.width) this.x = 0; if (this.x < 0) this.x = this.canvas.width;
    if (this.y > this.canvas.height) this.y = 0; if (this.y < 0) this.y = this.canvas.height;
    const dx = mouse.x - this.x; const dy = mouse.y - this.y;
    const distance = Math.sqrt(dx * dx + dy * dy);
    if (distance < mouse.radius) {
      if (mouse.x < this.x && this.x < this.canvas.width - this.size * 10) this.x += 2;
      if (mouse.x > this.x && this.x > this.size * 10) this.x -= 2;
      if (mouse.y < this.y && this.y < this.canvas.height - this.size * 10) this.y += 2;
      if (mouse.y > this.y && this.y > this.size * 10) this.y -= 2;
    }
  }
  draw(isDark: boolean) {
    this.ctx.fillStyle = isDark ? 'rgba(122, 186, 214, 0.7)' : 'rgba(47, 102, 133, 0.4)';
    this.ctx.beginPath(); this.ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2); this.ctx.fill();
  }
}
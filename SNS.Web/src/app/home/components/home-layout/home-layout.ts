import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { HomeNavbar } from '../home-navbar/home-navbar';
import { HomeBottomNavbar } from '../home-bottom-navbar/home-bottom-navbar';
import { HomeSideMenu } from '../home-side-menu/home-side-menu';

@Component({
  selector: 'app-home-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HomeNavbar, HomeBottomNavbar, HomeSideMenu],
  templateUrl: './home-layout.html',
  styleUrl: './home-layout.css'
})
export class HomeLayout {
  private breakpointObserver = inject(BreakpointObserver);

  isMobile = toSignal(
    this.breakpointObserver.observe('(max-width: 990px)').pipe(
      map(result => result.matches)
    ),
    { initialValue: false }
  );
}

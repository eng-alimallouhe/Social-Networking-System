import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { OverlayModule, ConnectedPosition } from '@angular/cdk/overlay';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { 
  LucideMenu, 
  LucideSearch, 
  LucideHome, 
  LucideFolderClosed, 
  LucideSquarePlus, 
  LucideMessagesSquare, 
  LucideBell,
  LucideFileText,
  LucideHelpCircle,
  LucideFolderPlus
} from '@lucide/angular';
import { HomeStateService } from '../../services/home-state.service';

@Component({
  selector: 'app-home-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    OverlayModule,
    TranslatePipe,
    LucideMenu,
    LucideSearch,
    LucideHome,
    LucideFolderClosed,
    LucideSquarePlus,
    LucideMessagesSquare,
    LucideBell,
    LucideFileText,
    LucideHelpCircle,
    LucideFolderPlus
  ],
  templateUrl: './home-navbar.html',
  styleUrl: './home-navbar.css'
})
export class HomeNavbar {
  private breakpointObserver = inject(BreakpointObserver);
  private homeState = inject(HomeStateService);
  private router = inject(Router);

  isCreateMenuOpen = signal(false);

  createMenuPositions = signal<ConnectedPosition[]>([
    {
      originX: 'end',
      originY: 'bottom',
      overlayX: 'end',
      overlayY: 'top',
      offsetY: 8
    },
    {
      originX: 'start',
      originY: 'bottom',
      overlayX: 'start',
      overlayY: 'top',
      offsetY: 8
    }
  ]);

  isMobile = toSignal(
    this.breakpointObserver.observe('(max-width: 990px)').pipe(
      map(result => result.matches)
    ),
    { initialValue: false }
  );

  toggleSideMenu() {
    this.homeState.toggleSideMenu();
  }

  navigateTo(route: string): void {
    this.isCreateMenuOpen.set(false);
    this.router.navigate([route]);
  }
}

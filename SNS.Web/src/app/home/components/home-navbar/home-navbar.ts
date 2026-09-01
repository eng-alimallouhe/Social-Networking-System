import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideMenu, LucideSearch, LucideHome, LucidePieChart, LucideMessageSquare, LucideFolderClosed, LucideSettings, LucidePlus, LucideBell, LucideSquarePlus, LucideMessagesSquare } from '@lucide/angular';
import { HomeStateService } from '../../services/home-state.service';

@Component({
  selector: 'app-home-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    TranslatePipe,
    LucideMenu,
    LucideSearch,
    LucideHome,
    LucideFolderClosed,
    LucideSquarePlus,
    LucideMessagesSquare,
    LucideBell
  ],
  templateUrl: './home-navbar.html',
  styleUrl: './home-navbar.css'
})
export class HomeNavbar {
  private breakpointObserver = inject(BreakpointObserver);
  private homeState = inject(HomeStateService);

  isMobile = toSignal(
    this.breakpointObserver.observe('(max-width: 990px)').pipe(
      map(result => result.matches)
    ),
    { initialValue: false }
  );

  toggleSideMenu() {
    this.homeState.toggleSideMenu();
  }
}

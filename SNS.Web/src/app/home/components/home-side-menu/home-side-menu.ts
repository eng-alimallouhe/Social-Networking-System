import { Component, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideX, LucideUser, LucideSettings, LucideLogOut } from '@lucide/angular';
import { HomeStateService } from '../../services/home-state.service';
import { PageService } from '../../../shared/services/page.service';

@Component({
  selector: 'app-home-side-menu',
  standalone: true,
  // تمت إزالة OverlayModule
  imports: [CommonModule, RouterLink, RouterLinkActive, TranslatePipe, LucideX, LucideUser, LucideSettings, LucideLogOut],
  templateUrl: './home-side-menu.html',
  styleUrl: './home-side-menu.css'
})
export class HomeSideMenu {
  public homeState = inject(HomeStateService);
  private pageService = inject(PageService);

  constructor() {
    effect(() => {
      // يفضل تفعيل منع التمرير فقط إذا كنا على شاشة الموبايل، يمكنك التحقق من ذلك أو تركها كما هي
      if (this.homeState.isSideMenuOpen()) {
        this.pageService.disableScroll();
      } else {
        this.pageService.enableScroll();
      }
    });
  }

  closeMenu() {
    this.homeState.closeSideMenu();
  }
}
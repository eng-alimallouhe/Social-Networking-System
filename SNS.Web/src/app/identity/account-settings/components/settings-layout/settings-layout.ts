import { Component, computed, inject, signal, OnDestroy, OnInit, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { rxResource } from '@angular/core/rxjs-interop';
import { Router, NavigationEnd, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { of, map, finalize, filter } from 'rxjs';
import { SkeletonLoaderComponent, SkeletonType } from '../../../../shared/components/loaders/skeleton-loader/skeleton-loader';
import { LoadingSettingsService } from '../../services/loading-settings.service';
import { UserAccount } from '../../../users/user-management/contracts/user-account.dto';
import { UserManagementService } from '../../../users/user-management/services/user-management.service';
import { CircleLoader } from '../../../../shared/components/loaders/circle-loader/circle-loader';
import { LineLoader } from '../../../../shared/components/loaders/line-loader/line-loader';
import { FormsModule } from '@angular/forms';
import { LucideSearch, LucideMenu, LucideX, LucideClock, LucideSearchX, LucideChevronRight } from '@lucide/angular';
import { SettingsSearchService, GroupedSettings } from '../../services/settings-search.service';
import { SettingEntry, SETTINGS_CONFIG } from '../../settings.config';
import { HighlightPipe } from '../../pipes/highlight.pipe';

@Component({
  selector: 'app-settings-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    TranslatePipe,
    RouterLink,
    RouterLinkActive,
    LineLoader,
    SkeletonLoaderComponent,
    FormsModule,
    LucideSearch,
    LucideMenu,
    LucideX,
    LucideClock,
    LucideSearchX,
    LucideChevronRight,
    HighlightPipe
  ],
  templateUrl: './settings-layout.html',
  styleUrl: './settings-layout.css',
})
export class SettingsLayout implements OnInit {
  private userManagementService = inject(UserManagementService);
  private router = inject(Router);
  private translateService = inject(TranslateService);
  private loadingService = inject(LoadingSettingsService);
  private searchService = inject(SettingsSearchService);
  private accountCach: UserAccount | null = null;

  public searchQuery = signal('');
  public isSearchFocused = signal(false);
  public isMobileNavOpen = signal(false);

  public isLoadingSettings = this.loadingService.isLoadingSettings;
  public readonly skeletonType = SkeletonType;

  // The complete layout settings for the sidebar navigation
  public allSettings = this.searchService.getAllSettings();
  public homeSetting = this.allSettings.find(s => s.id === 'account-home');
  public navSettings = this.allSettings.filter(s => s.id !== 'account-home' && s.id !== 'account-archive');
  public archiveSettings = this.allSettings.filter(s => s.id === 'account-archive');

  userResource = rxResource({
    params: () => ({
      isRoot: this.isRootRoute()
    }),
    stream: ({ params }) => {
      if (!params?.isRoot || this.accountCach !== null) {
        return of(this.accountCach);
      }

      this.loadingService.show();
      return this.userManagementService.getUserAccounts().pipe(
        map(result => {
          this.accountCach = result.value;
          return result.value;
        }),
        finalize(() => {
          this.loadingService.hide();
        })
      );
    }
  });

  searchResults = computed(() => {
    return this.searchService.searchSettings(this.searchQuery());
  });

  recentSettings = computed(() => {
    // Only show recent if search is empty and focused
    if (this.searchQuery().trim() === '' && this.isSearchFocused()) {
      return this.searchService.getRecentSettings();
    }
    return [];
  });

  isRootRoute = signal(true);

  private anchorToScroll: string | null = null;

  constructor() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.isRootRoute.set(event.urlAfterRedirects.split('?')[0].split('#')[0] === '/account-settings');
      this.isMobileNavOpen.set(false); // Close mobile nav on navigate

      // Handle anchor scrolling
      const tree = this.router.parseUrl(this.router.url);
      if (tree.fragment) {
        this.anchorToScroll = tree.fragment;
        this.scrollToAnchor();
      }
    });
  }

  ngOnInit(): void {
  }

  toggleMobileNav(): void {
    this.isMobileNavOpen.set(!this.isMobileNavOpen());
  }

  onSearchFocus(): void {
    this.isSearchFocused.set(true);
  }

  onSearchBlur(): void {
    // Delay hiding to allow clicking on results
    setTimeout(() => {
      this.isSearchFocused.set(false);
    }, 200);
  }

  selectSetting(setting: SettingEntry): void {
    this.searchService.addRecentSetting(setting);
    this.searchQuery.set('');

    const extras = setting.anchor ? { fragment: setting.anchor } : {};
    this.router.navigate([setting.route], extras);
  }

  goHome(): void {
    this.router.navigate(['/account-settings']);
  }

  private scrollToAnchor(): void {
    if (!this.anchorToScroll) return;

    setTimeout(() => {
      const element = document.getElementById(this.anchorToScroll!);
      if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'center' });
        element.classList.add('highlight-target');
        setTimeout(() => element.classList.remove('highlight-target'), 2000);
        this.anchorToScroll = null;
      }
    }, 100);
  }
}

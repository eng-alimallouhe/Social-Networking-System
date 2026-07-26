import { Component, computed, effect, inject, signal } from '@angular/core';
import { UserManagementService } from '../../../users/user-management/services/user-management.service';
import { UserAccount } from '../../../users/user-management/contracts/user-account.dto';
import { NavigationEnd, Router, RouterLink, RouterOutlet, RouterLinkActive } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { LucideArchive, LucideFingerprintPattern, LucideHouse, LucideIdCard, LucideKeyRound, LucideShieldPlus } from "@lucide/angular";
import { filter, finalize, map, of } from 'rxjs';
import { CircleLoaderComponent } from "../../../../shared/components/loaders/circle-loader/circle-loader.component";
import { Result } from '../../../../shared/contracts/result';
import { SettingItem } from '../../contracts/setting-item.dto';
import { DynamicIconComponent } from "../../../../shared/components/dynamic-icon/dynamic-icon.component";
import { rxResource } from '@angular/core/rxjs-interop';
import { LoadingSettingsService } from '../../services/loading-settings.service';
import { LinearLoaderComponent } from "../../../../shared/components/loaders/linear-loader/linear-loader.component";
import { SkeletonLoaderComponent } from "../../../../shared/components/loaders/skeleton-loader/skeleton-loader.component";
import { SkeletonType } from '../../../../shared/components/loaders/skeleton-loader/skeleton-loader.types';

@Component({
  selector: 'app-settings-layout',
  imports: [
    CircleLoaderComponent,
    RouterOutlet,
    TranslateModule,
    RouterLink,
    LucideHouse,
    LucideIdCard,
    LucideFingerprintPattern,
    LucideKeyRound,
    LucideArchive,
    LucideShieldPlus,
    CircleLoaderComponent,
    RouterLinkActive,
    DynamicIconComponent,
    LinearLoaderComponent,
    SkeletonLoaderComponent
  ],
  templateUrl: './settings-layout.component.html',
  styleUrl: './settings-layout.component.css',
})
export class SettingsLayoutComponent {
  private userManagementService = inject(UserManagementService);
  private router = inject(Router);
  private translateService = inject(TranslateService);
  private loadingService = inject(LoadingSettingsService);
  private accountCach: UserAccount | null = null;

  public searchQuery = signal('');
  public isLoadingSettings = this.loadingService.isLoadingSettings;
  public readonly skeletonType = SkeletonType;


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

  settingsList: SettingItem[] = [
    {
      icon: "IdCard",
      title: "Personal Information",
      accessGuide: "Manage your personal details.",
      keywords: ["Personal", "Information", "Profile", "Details"],
      route: "/account-settings/personal-information"
    },
    {
      icon: "<svg lucideFingerprintPattern></svg>",
      title: "Security Settings",
      accessGuide: "Manage your security settings.",
      keywords: ["Security", "Settings"],
      route: "/account-settings/security-settings"
    },
    {
      icon: "<svg lucideKeyRound></svg>",
      title: "Password Management",
      accessGuide: "Manage your password.",
      keywords: ["Password", "Management"],
      route: "/account-settings/password-management"
    },
    {
      icon: "<svg lucideShieldPlus></svg>",
      title: "Sessions",
      accessGuide: "Manage your sessions.",
      keywords: ["Sessions"],
      route: "/account-settings/sessions"
    },
    {
      icon: "<svg lucideArchive></svg>",
      title: "Archive",
      accessGuide: "Manage your archive.",
      keywords: ["Archive"],
      route: "/account-settings/archive"
    }
  ];

  filteredSettings = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return [];

    return this.settingsList.filter(item =>
      item.keywords.some((k: string) => k.toLowerCase().includes(query)) ||
      item.title.toLowerCase().includes(query)
    );
  });

  isRootRoute = signal(true);
  isLoadingResult = signal(true);

  constructor() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.isRootRoute.set(event.urlAfterRedirects === '/account-settings' || event.urlAfterRedirects === '/');
    });
  }

  ngOnInit(): void {

    var keys = this.settingsList.flatMap(s => `Identity.Security_Settings.Search.${s.keywords}`);

    this.translateService.get(keys)
      .subscribe((translations: any) => {
        this.settingsList = this.settingsList.map((s: SettingItem) => {
          return {
            ...s,
            keywords: s.keywords.map((k: string) => translations[k] || k)
          }
        })
      });
  }
}
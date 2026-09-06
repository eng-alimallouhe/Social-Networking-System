import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideHome, LucideSearch, LucidePieChart, LucideSettings, LucideFolderGit2, LucideMessagesSquare, LucideUser } from '@lucide/angular';

@Component({
  selector: 'app-home-bottom-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    TranslatePipe,
    LucideHome,
    LucideSearch,
    LucideFolderGit2,
    LucideUser,
    LucideMessagesSquare
  ],
  templateUrl: './home-bottom-navbar.html',
  styleUrl: './home-bottom-navbar.css'
})
export class HomeBottomNavbar {
  navItems = [
    { path: '/home', icon: 'home', labelKey: 'App.Layout.Home', exact: true },
    { path: '/home/search', icon: 'search', labelKey: 'App.Layout.Search', exact: false },
    { path: '/home/projects', icon: 'folder-git-2', labelKey: 'App.Layout.Projects', exact: false },
    { path: '/home/discussion', icon: 'messages-square', labelKey: 'App.Layout.Forum', exact: false },
    { path: '/home/profile', icon: 'user', labelKey: 'App.Layout.Profile', exact: false },
  ];
}

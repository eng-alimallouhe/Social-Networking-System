import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { LucideSearch, LucideAlertCircle, LucideArrowRight, LucideChevronDown, LucideChevronUp } from '@lucide/angular';
import { DEMO_CONFIG, DemoSection } from '../../demo.config';
import { DemoDataService } from '../../services/demo-data.service';

@Component({
  selector: 'app-demo-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe, FormsModule, LucideSearch, LucideAlertCircle, LucideArrowRight, LucideChevronDown, LucideChevronUp],
  templateUrl: './demo-dashboard.html',
  styleUrl: './demo-dashboard.css',
})
export class DemoDashboard {
  private demoDataService = inject(DemoDataService);
  private translateService = inject(TranslateService);

  searchQuery = signal('');
  collapsedSections = signal<Set<string>>(new Set<string>(DEMO_CONFIG.map(section => section.titleKey)));

  toggleSection(sectionKey: string) {
    this.collapsedSections.update(set => {
      const newSet = new Set(set);
      if (newSet.has(sectionKey)) {
        newSet.delete(sectionKey);
      } else {
        newSet.add(sectionKey);
      }
      return newSet;
    });
  }

  // Map the config to resolve dynamic query parameters once
  private baseSections = signal<DemoSection[]>(
    DEMO_CONFIG.map(section => ({
      ...section,
      pages: section.pages.map(page => ({
        ...page,
        queryParams: page.generateQueryParams ? page.generateQueryParams(this.demoDataService) : page.queryParams
      }))
    }))
  );

  // Computed signal for filtered sections based on search query
  filteredSections = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    if (!query) {
      return this.baseSections();
    }

    return this.baseSections().map(section => {
      // We must check translations for matching since search happens in the user's current language
      const sectionTitle = this.translateService.instant(section.titleKey).toLowerCase();
      
      const filteredPages = section.pages.filter(page => {
        const title = this.translateService.instant(page.titleKey).toLowerCase();
        const desc = this.translateService.instant(page.descriptionKey).toLowerCase();
        return title.includes(query) || desc.includes(query) || sectionTitle.includes(query);
      });

      return { ...section, pages: filteredPages };
    }).filter(section => section.pages.length > 0);
  });

  // Computed total pages count
  totalPages = computed(() => {
    return this.baseSections().reduce((total, section) => total + section.pages.length, 0);
  });
}

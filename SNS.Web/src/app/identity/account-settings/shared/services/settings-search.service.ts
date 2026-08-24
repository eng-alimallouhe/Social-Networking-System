import { inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { SettingEntry, SETTINGS_CONFIG } from '../contracts/settings.config';

export interface GroupedSettings {
    categoryKey: string;
    items: SettingEntry[];
}

@Injectable({
    providedIn: 'root'
})
export class SettingsSearchService {
    private translate = inject(TranslateService);

    // Track recently visited settings in local storage

    // using a custom storage key or standard mechanism.
    private readonly RECENT_SETTINGS_KEY = 'recent_settings_ids';

    getAllSettings(): SettingEntry[] {
        return SETTINGS_CONFIG;
    }

    getGroupedSettings(): GroupedSettings[] {
        return this.groupSettings(SETTINGS_CONFIG);
    }

    searchSettings(query: string): GroupedSettings[] {
        if (!query || query.trim() === '') {
            return [];
        }

        const lowerQuery = query.toLowerCase().trim();

        const filtered = SETTINGS_CONFIG.filter(setting => {
            const title = this.translate.instant(setting.titleKey).toLowerCase();
            const desc = this.translate.instant(setting.descriptionKey).toLowerCase();
            const category = this.translate.instant(setting.categoryKey).toLowerCase();

            // Check keywords (translate them if they are keys, else just check raw)
            // But we already map translations in the layout or we can do it here.
            const matchesKeywords = setting.keywords.some(k => k.toLowerCase().includes(lowerQuery));

            return title.includes(lowerQuery) || desc.includes(lowerQuery) || category.includes(lowerQuery) || matchesKeywords;
        });

        return this.groupSettings(filtered);
    }

    getRecentSettings(): SettingEntry[] {
        try {
            const stored = localStorage.getItem(this.RECENT_SETTINGS_KEY);
            if (stored) {
                const ids: string[] = JSON.parse(stored);
                return ids.map(id => SETTINGS_CONFIG.find(s => s.id === id))
                    .filter(s => s !== undefined) as SettingEntry[];
            }
        } catch (e) {
        }
        return [];
    }

    addRecentSetting(setting: SettingEntry): void {
        const recents = this.getRecentSettings();
        // Remove if already exists to move it to top
        const filtered = recents.filter(s => s.id !== setting.id);
        filtered.unshift(setting);

        // Keep only top 3
        const top3 = filtered.slice(0, 3);
        localStorage.setItem(this.RECENT_SETTINGS_KEY, JSON.stringify(top3.map(s => s.id)));
    }

    private groupSettings(settings: SettingEntry[]): GroupedSettings[] {
        const groups = new Map<string, SettingEntry[]>();
        settings.forEach(setting => {
            if (!groups.has(setting.categoryKey)) {
                groups.set(setting.categoryKey, []);
            }
            groups.get(setting.categoryKey)!.push(setting);
        });

        return Array.from(groups.entries()).map(([categoryKey, items]) => ({
            categoryKey,
            items
        }));
    }
}

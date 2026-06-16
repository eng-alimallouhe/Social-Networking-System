export interface ProfileSearchQuery {
    searchTerm?: string;
    requiredSkills?: string[];
    currentProfileId?: number;
    page: number;
    size: number;
}
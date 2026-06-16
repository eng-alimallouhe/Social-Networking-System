import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideTrendingUp, LucideSearch, LucideUsers, LucideBriefcase, LucideFolderKanban, LucideMessageSquare, LucideHistory, LucideLightbulb, LucideTarget, LucideMapPin, LucideDollarSign, LucideClock, LucideListFilter, LucideX } from '@lucide/angular';
import { TranslatePipe } from '@ngx-translate/core';
import { CommunitySummaryDto } from '../../../communities/dtos/community-summary.dto';
import { PostSummaryDto } from '../../../content/posts/dtos/post-summary.dto';
import { JobSummaryDto } from '../../../jobs/dtos/job-summary.dto';
import { ProjectSummaryDto } from '../../../projects/dtos/project-summary.dto';
import { ProfileSummaryDto } from '../../../social-graph/dtos/profile-summary.dto';
import { ProfileFiltersComponent } from "../../shared/components/profile-filters/profile-filters.component";
import { ProjectsFiltersComponent } from '../../shared/components/projects-filters/projects-filters.component';
import { ProblemsFiltersComponent } from '../../shared/components/problems-filters/problems-filters.component';
import { JobsFiltersComponent } from '../../shared/components/jobs-filters/jobs-filters.component';
import { CommunitiesFiltersComponent } from '../../shared/components/communities-filters/communities-filters.component';
import { PostsFiltersComponent } from '../../shared/components/posts-filters/posts-filters.component';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe,
    LucideSearch,
    LucideTrendingUp,
    LucideHistory,
    LucideLightbulb,
    LucideTarget,
    LucideListFilter,
    LucideX,
    ProfileFiltersComponent,
    ProjectsFiltersComponent,
    ProblemsFiltersComponent,
    JobsFiltersComponent,
    CommunitiesFiltersComponent,
    PostsFiltersComponent
  ],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css'
})
export class SearchComponent {
  public isFiltersPanalOpen = signal(false);
  public activeCategory = signal(SearchCategory.All);

  public categories = [
    SearchCategory.All,
    SearchCategory.People,
    SearchCategory.Posts,
    SearchCategory.Projects,
    SearchCategory.Communities,
    SearchCategory.Jobs,
    SearchCategory.Problems
  ];

  public resultProjects = signal<ProjectSummaryDto[]>([]);
  public resultPosts = signal<PostSummaryDto[]>([]);
  public resultPeople = signal<ProfileSummaryDto[]>([]);
  public resultCommunities = signal<CommunitySummaryDto[]>([]);
  public resultJobs = signal<JobSummaryDto[]>([]);
  public resultQnA = signal<ProfileSummaryDto[]>([]);

  public filters = [
    {
      name: 'All',
      filters: [],
    },
    {
      name: 'People',
      filters: [
        {
          'label': '',
          'icon': '',
          'type': '',
          'options': [
            {
              'label': '',
              'value': ''
            }
          ]
        }
      ]
    }
  ];

  public recentSearches = ['Machine Learning Ethics', 'Stanford Alumni 2023', 'Web3 Research Grant'];
  public trendingTopics = ['Large Language Models', 'Fusion Energy Breakthrough', 'Bioinformatics Tooling'];
  public suggestedForYou = ['Physics Communities', 'Open Source Papers', 'Local Meetups'];

  setCategory(cat: SearchCategory) {
    this.activeCategory.set(cat);
  }

  toggleFilters() {
    this.isFiltersPanalOpen.update(v => !v);
  }
}

export enum SearchCategory {
  All = 0,
  People = 1,
  Posts = 2,
  Projects = 3,
  Communities = 4,
  Jobs = 5,
  Problems = 6
}

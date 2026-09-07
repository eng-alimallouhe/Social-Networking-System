import { Component, Input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectOverviewDto } from '../../contracts/project-summary.dto';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideExternalLink, LucideStar, LucideUsers, LucideGlobe, LucideSave, LucideGitFork } from "@lucide/angular";
import { RouterLink } from "@angular/router";
import { ProjectStatus } from '../../enums/project-status.enum';
import { ProjectType } from '../../enums/project-type.enum';

import { AppAvatar } from '../../../shared/design-system/components/app-avatar/app-avatar';

@Component({
  selector: 'app-project',
  imports: [
    CommonModule,
    TranslatePipe, 
    LucideStar, 
    LucideExternalLink, 
    RouterLink, 
    LucideGlobe, 
    LucideSave,
    LucideGitFork,
    AppAvatar
  ],
  templateUrl: './project.html',
  styleUrl: './project.css',
})
export class Project {
  @Input({ required: true }) project!: ProjectOverviewDto;
  projectClicked = output<string>();

  onProjectClick(event?: Event): void {
    if (event) {
      event.preventDefault();
    }
    const id = this.project?.id || (this.project as any)?.projectId || '';
    if (id) {
      this.projectClicked.emit(id);
    }
  }

  getSkillName(skill: any): string {
    if (!skill) return '';
    if (typeof skill === 'string') return skill;
    return skill.skillName || skill.name || '';
  }

  ngOnInit(): void {
    if (!this.project) {
      this.project = {
        id: '1',
        title: 'Social Media Network',
        shortDescription: 'A social media network for professionals to connect and share their work. This is a project that I worked on for a class. It is a social media network for professionals to connect and share their work.',
        createdAt: new Date(),
        gitHubUrl: 'https://github.com/eng-alimallouhe/Social-Networking-System',
        liveDemoUrl: 'https://www.syrian-developers.sy',
        ratingsCount: 12354,
        averageRating: 9.2,
        status: ProjectStatus.Ongoing,
        type: ProjectType.OpenSource,
        participants: [
          {
            profileId: '1',
            profileImageUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sarah'
          },
          {
            profileId: '2',
            profileImageUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sarah'
          },
          {
            profileId: '3',
            profileImageUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sarah',
          },
        ],
        participantsCount: 5,
        skills: [
          "full stack developer",
          "backend developer",
          "AI Engineer",
        ],
        skillsCount: 5,
        savesCount: 123
      };
    }
  }
}

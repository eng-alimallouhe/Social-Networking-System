import { Component, Input } from '@angular/core';
import { ProjectSummaryDto } from '../../dtos/project-summary.dto';
import { TranslateModule } from '@ngx-translate/core';
import { LucideExternalLink, LucideStar, LucideUsers, LucideGlobe, LucideSave } from "@lucide/angular";
import { RouterLink } from "@angular/router";
import { GithubIconComponent } from "../../../shared/Components/icons/github-icon/github-icon.component";
import { ProjectStatus } from '../../enums/project-status.enum';
import { ProjectType } from '../../enums/project-type.enum';

@Component({
  selector: 'app-project',
  imports: [TranslateModule, LucideStar, LucideExternalLink, RouterLink, GithubIconComponent, LucideGlobe, LucideSave],
  templateUrl: './project.component.html',
  styleUrl: './project.component.css',
})
export class ProjectComponent {
  @Input({ required: true }) project!: ProjectSummaryDto;

  ngOnInit(): void {
    this.project = {
      id: '1',
      ownerId: '1',
      title: 'Social Media Network',
      shortDescription: 'A social media network for professionals to connect and share their work. This is a project that I worked on for a class. It is a social media network for professionals to connect and share their work.',
      createdAt: new Date(),
      updatedAt: new Date(),
      gitHubUrl: 'https://github.com/eng-alimallouhe/Social-Networking-System',
      liveUrl: 'https://www.syrian-developers.sy',
      publishedAt: new Date(),
      rate: 9.2,
      totalRates: 1235,
      status: ProjectStatus.Ongoing,
      type: ProjectType.OpenSource,
      topThreeContributors: [
        {
          id: '1',
          fullName: 'Ali Mallouhe',
          profilePictureUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sarah',
          specialization: 'full stack developer'
        },
        {
          id: '2',
          fullName: 'Abdulallah Salem',
          profilePictureUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sarah',
          specialization: 'backend developer'
        },
        {
          id: '3',
          fullName: 'Mohammad Mahson',
          profilePictureUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sarah',
          specialization: 'AI Engineer'
        },
      ],
      contributorsCount: 5,
      topThreeSkills: [
        "full stack developer",
        "backend developer",
        "AI Engineer",
      ],
      skillsCount: 5,
      savesCount: 123
    };
  }
}

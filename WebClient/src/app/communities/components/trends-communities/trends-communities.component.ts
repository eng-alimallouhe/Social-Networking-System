import { Component, inject, OnInit } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { CommunityService } from '../../services/community.service';
import { Observable } from 'rxjs';
import { CommunitySummaryDto } from '../../dtos/community-summary.dto';
import { AsyncPipe } from '@angular/common';
import { CompactNumberPipe } from "../../../shared/Pipes/compact-number-pipe";
import { RouterLink } from "@angular/router";
import { UpperCasePipe } from '@angular/common';
import { LucideAArrowDown, LucideArrowRight, LucideChevronRight } from "@lucide/angular";

@Component({
  selector: 'app-trends-communities',
  imports: [TranslatePipe, AsyncPipe, CompactNumberPipe, RouterLink, UpperCasePipe, 
    LucideArrowRight,
    LucideAArrowDown, LucideChevronRight],
  templateUrl: './trends-communities.component.html',
  styleUrl: './trends-communities.component.css',
})
export class TrendsCommunitiesComponent implements OnInit {
  private communityService = inject(CommunityService);

  public trendsCommunities$!: Observable<CommunitySummaryDto[]>;

  ngOnInit(): void {
    this.trendsCommunities$ = this.communityService.getTrendingCommunities();
  }
}

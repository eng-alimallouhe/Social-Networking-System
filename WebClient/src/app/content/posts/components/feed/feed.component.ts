import { Component, inject, Signal, signal } from '@angular/core';
import { Observable, finalize } from 'rxjs';
import { PostSummaryDto } from '../../dtos/post-summary.dto';
import { PostService } from '../../services/post.service';
import { PostComponent } from "../post/post.component";
import { AsyncPipe, CommonModule } from '@angular/common';
import { TrendsCommunitiesComponent } from "../../../../communities/components/trends-communities/trends-communities.component";
import { CreationHubComponent } from "../creation-hub/creation-hub.component";
import { SuggestedFollowingsComponent } from "../../../../social-graph/components/suggested-followings/suggested-followings.component";
import { AuthenticationService } from '../../../../Identity/Authentication/Services/authentication.service';
import { ProfileService } from '../../../../social-graph/services/profile.service';
import { TrackVisibilityDirective } from "../../../../shared/directives/track-visibility.directive";

@Component({
  selector: 'app-feed',
  imports: [
    PostComponent,
    AsyncPipe,
    CommonModule,
    TrendsCommunitiesComponent,
    CreationHubComponent,
    SuggestedFollowingsComponent,
    TrackVisibilityDirective
],
  templateUrl: './feed.component.html',
  styleUrl: './feed.component.css',
})
export class FeedComponent {
  private authenticationService = inject(AuthenticationService);
  private profileService = inject(ProfileService);

  posts$: Observable<PostSummaryDto[]> | undefined;

  profileId: Signal<string | null> = this.profileService.profileId;
  
  isLoadingFeed: boolean = true;
  userAvatarUrl: string = "https://api.dicebear.com/7.x/avataaars/svg?seed=Sarah";
  isAuthenticated!: Signal<boolean>;

  constructor(
    private postService: PostService
  ) {
    this.isAuthenticated = this.authenticationService.isAuthenticated;
  }

  ngOnInit(): void {
    this.posts$ = this.postService.getUserFeed().pipe(
      finalize(() => {
        this.isLoadingFeed = false;
      })
    );
  }
}

import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';
import { LucideArrowRight, LucideUserPlus } from '@lucide/angular';
import { FollowService } from '../../services/follow.service';
import { Observable } from 'rxjs';
import { ProfileBaseDto } from '../../dtos/profile-base-dto.dto';


@Component({
  selector: 'app-suggested-followings',
  standalone: true,
  imports: [CommonModule, 
    TranslatePipe,
    RouterLink,
    LucideArrowRight, 
    LucideUserPlus],
  templateUrl: './suggested-followings.component.html',
  styleUrl: './suggested-followings.component.css'
})
export class SuggestedFollowingsComponent {
  private followService = inject(FollowService);

  public suggestedFollowings$!: Observable<ProfileBaseDto[]>;

  ngOnInit() {
    this.suggestedFollowings$ = this.followService.getSuggestedFollowings();
  }
}

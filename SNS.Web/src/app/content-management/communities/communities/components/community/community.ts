import { Component, input, output, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { CommunitySummaryDto } from '../../contracts/community-summary.dto';
import { CommunityType } from '../../../../../shared/contracts/community-type';

import { getInitials } from '../../../../../shared/utils/avatar-utils';

@Component({
    selector: 'app-community',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        TranslatePipe
    ],
    templateUrl: './community.html',
    styleUrl: './community.css'
})
export class Community {
    community = input.required<CommunitySummaryDto>();
    communityClicked = output<string>();

    readonly CommunityType = CommunityType;

    initials = computed(() => {
        return getInitials(this.community().name);
    });

    isPublic = computed(() => {
        return this.community().type === CommunityType.Public;
    });

    onCommunityClick(event?: Event): void {
        if (event) {
            event.preventDefault();
        }
        this.communityClicked.emit(this.community().id);
    }
}

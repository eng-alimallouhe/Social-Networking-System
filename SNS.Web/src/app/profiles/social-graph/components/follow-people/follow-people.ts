import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { SuggestedUser } from '../../contracts/suggested-user';
import { rxResource } from '@angular/core/rxjs-interop';

const MOCK_SUGGESTED_USERS: SuggestedUser[] = [
  {
    id: '1',
    fullName: 'Ahmad Al-Hassan',
    username: 'ahmad.hassan',
    specialization: 'Backend Developer',
    bio: 'Senior Backend Developer passionate about distributed systems and open-source contributions.',
    followerCount: 12430,
    followingCount: 380,
    avatarColor: '#4f46e5'
  },
  {
    id: '2',
    fullName: 'Sara Al-Khalil',
    username: 'sara.dev',
    specialization: 'Full Stack Developer',
    bio: 'Full Stack Engineer specializing in React and Node.js. Building things that matter every day.',
    followerCount: 8750,
    followingCount: 214,
    avatarColor: '#0891b2'
  },
  {
    id: '3',
    fullName: 'Mohammed Idris',
    username: 'm.idris',
    specialization: 'DevOps Engineer',
    bio: 'DevOps & Cloud Engineer. Kubernetes, Terraform, and everything in between the pipeline.',
    followerCount: 5200,
    followingCount: 167,
    avatarColor: '#059669'
  },
  {
    id: '4',
    fullName: 'Lina Barakat',
    username: 'lina.ux',
    specialization: 'UX Designer',
    bio: 'UX Designer crafting intuitive and accessible digital experiences for complex products.',
    followerCount: 19800,
    followingCount: 502,
    avatarColor: '#db2777'
  },
  {
    id: '5',
    fullName: 'Kareem Nasser',
    username: 'kareem.ai',
    specialization: 'Machine Learning Engineer',
    bio: 'ML Engineer focused on NLP and large language models. Contributor to open-source AI tools.',
    followerCount: 34100,
    followingCount: 89,
    avatarColor: '#7c3aed'
  },
  {
    id: '6',
    fullName: 'Rania Saleh',
    username: 'rania.mobile',
    specialization: 'Flutter Developer',
    bio: 'Flutter Developer building cross-platform apps for both iOS and Android with clean architecture.',
    followerCount: 6400,
    followingCount: 310,
    avatarColor: '#0284c7'
  },
  {
    id: '7',
    fullName: 'Firas Othman',
    username: 'firas.sec',
    specialization: 'Cybersecurity Engineer',
    bio: 'Cybersecurity Engineer. CTF enthusiast, ethical hacker, and security researcher.',
    followerCount: 9100,
    followingCount: 143,
    avatarColor: '#b45309'
  },
  {
    id: '8',
    fullName: 'Nour Khaled',
    username: 'nour.data',
    specialization: 'Data Engineer',
    bio: 'Data Engineer building reliable data pipelines and modern analytics platforms at scale.',
    followerCount: 4300,
    followingCount: 255,
    avatarColor: '#c026d3'
  },
  {
    id: '9',
    fullName: 'Yasir Mansoor',
    username: 'yasir.cloud',
    specialization: 'Solutions Architect',
    bio: 'Cloud Solutions Architect with AWS and Azure certifications. Designing scalable systems.',
    followerCount: 15600,
    followingCount: 420,
    avatarColor: '#be123c'
  },
  {
    id: '10',
    fullName: 'Hala Zein',
    username: 'hala.qa',
    specialization: 'QA Engineer',
    bio: 'QA Automation Engineer. Ensuring software quality through rigorous testing strategies.',
    followerCount: 3800,
    followingCount: 198,
    avatarColor: '#1d4ed8'
  }
];

const MIN_FOLLOWS = 5;

@Component({
  selector: 'app-follow-people',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './follow-people.html',
  styleUrl: './follow-people.css'
})
export class FollowPeople {
  private router = inject(Router);

  readonly users = signal<SuggestedUser[]>(MOCK_SUGGESTED_USERS);
  readonly followedIds = signal<Set<string>>(new Set());
  readonly minFollows = MIN_FOLLOWS;

  followedCount = computed(() => this.followedIds().size);
  canContinue = computed(() => this.followedCount() >= MIN_FOLLOWS);

  // suggestedFollowingsResource = rxResource({
  //   params: () => ({

  //   }),
  //   stream: ({ params }) => {

  //   }
  // });

  progressPercent = computed(() =>
    Math.min((this.followedCount() / MIN_FOLLOWS) * 100, 100)
  );

  isFollowing(userId: string): boolean {
    return this.followedIds().has(userId);
  }

  toggleFollow(userId: string): void {
    this.followedIds.update(current => {
      const next = new Set(current);
      if (next.has(userId)) {
        next.delete(userId);
      } else {
        next.add(userId);
      }
      return next;
    });
  }

  getInitials(fullName: string): string {
    return fullName
      .split(' ')
      .slice(0, 2)
      .map(n => n[0])
      .join('')
      .toUpperCase();
  }

  /** Compact count formatter: 12430 → "12.4K", 1200000 → "1.2M" */
  formatCount(count: number): string {
    if (count >= 1_000_000) {
      return (count / 1_000_000).toFixed(1).replace(/\.0$/, '') + 'M';
    }
    if (count >= 1_000) {
      return (count / 1_000).toFixed(1).replace(/\.0$/, '') + 'K';
    }
    return count.toString();
  }

  onContinue(): void {
    // Home redirection will be implemented later
    this.router.navigate(['/']);
  }
}

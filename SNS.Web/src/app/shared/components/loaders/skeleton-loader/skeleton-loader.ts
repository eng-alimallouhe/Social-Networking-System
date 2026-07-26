import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-skeleton-loader',
  imports: [CommonModule],
  templateUrl: './skeleton-loader.html',
  styleUrl: './skeleton-loader.css',
})
export class SkeletonLoaderComponent {
  type = input<SkeletonType>(SkeletonType.AccountOnPhone);
  width = input<string>('100%');
  height = input<string>('auto');

  readonly SkeletonType = SkeletonType;
}

export enum SkeletonType {
  AccountOnPhone = 'AccountOnPhone',
  AccountOnDesktop = 'AccountOnDesktop',
  Post = 'Post',
  Comment = 'Comment',
}

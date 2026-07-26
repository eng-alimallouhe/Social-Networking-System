import { Component, effect, Input, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonType } from './skeleton-loader.types';

@Component({
  selector: 'app-skeleton-loader',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './skeleton-loader.component.html',
  styleUrl: './skeleton-loader.component.css',
})
export class SkeletonLoaderComponent {
  readonly SkeletonType = SkeletonType;
  @Input() type: SkeletonType = SkeletonType.AccountOnPhone;
  @Input() width: string = '300px';
  @Input() height: string = '75px';

  constructor() {
    console.log('type =', this.type);
    console.log('enum =', this.SkeletonType.AccountOnPhone);
  }
}

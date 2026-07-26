import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-simple-circle-loader',
  imports: [CommonModule],
  templateUrl: './simple-circle-loader.component.html',
  styleUrl: './simple-circle-loader.component.css',
})
export class SimpleCircleLoaderComponent {
  @Input({ required: true }) width!: string;
  @Input({ required: true }) height!: string;
}

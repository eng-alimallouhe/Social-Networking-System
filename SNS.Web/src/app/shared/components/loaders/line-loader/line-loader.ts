import { CommonModule } from '@angular/common';
import { Component, input } from '@angular/core';

@Component({
  selector: 'app-line-loader',
  imports: [CommonModule],
  templateUrl: './line-loader.html',
  styleUrl: './line-loader.css',
})
export class LineLoader {
  width = input<string>('100%');
}

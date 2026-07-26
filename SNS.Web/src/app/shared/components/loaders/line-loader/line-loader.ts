import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-line-loader',
  imports: [CommonModule],
  templateUrl: './line-loader.html',
  styleUrl: './line-loader.css',
})
export class LineLoader {
  @Input() width: string = '100%';
}
